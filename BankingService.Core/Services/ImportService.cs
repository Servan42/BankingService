﻿using BankingService.Core.API.Interfaces;
using BankingService.Core.Model;
using BankingService.Core.SPI.DTOs;
using BankingService.Core.SPI.Interfaces;
using Microsoft.VisualBasic.FileIO;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Core.Services
{
    public class ImportService : IImportService
    {
        private const string BANK_ARCHIVE_FOLDER = "Archive/Bank_Import";
        private const string PAYPAL_ARCHIVE_FOLDER = "Archive/Paypal_Import";

        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IFileSystemService fileSystemService;
        private readonly IBankDatabaseService bankDatabaseService;

        public ImportService(IFileSystemService fileSystemService, IBankDatabaseService bankDatabaseService)
        {
            this.fileSystemService = fileSystemService;
            this.bankDatabaseService = bankDatabaseService;
        }

        public void ImportBankFile(string bankFilePath)
        {
            logger.Info($"Importing {bankFilePath} bank file");
            var csvOperations = fileSystemService.ReadAllLines(bankFilePath);
            List<Operation> operations = GetBankOperationsFromCSV(csvOperations);
            ResolveOperationsAutoFields(operations);
            bankDatabaseService.InsertOperationsIfNew(operations.Select(o => o.MapToDto()).ToList());
            fileSystemService.ArchiveFile(bankFilePath, BANK_ARCHIVE_FOLDER);
        }

        private List<Operation> GetBankOperationsFromCSV(List<string> csvOperations)
        {
            var operations = new List<Operation>();

            foreach (var csvOperation in csvOperations.Skip(1))
            {
                var splitedOperation = csvOperation.Split(";");
                operations.Add(new Operation
                {
                    Date = DateTime.Parse(splitedOperation[0]),
                    Flow = GetBankFlow(splitedOperation),
                    Label = splitedOperation[4],
                    Treasury = decimal.Parse(splitedOperation[5])
                });
            }

            return operations;
        }

        private void ResolveOperationsAutoFields(List<Operation> operations)
        {
            var operationTypes = bankDatabaseService.GetOperationTypesKvp();
            var operationCategoriesAndAutoComment = bankDatabaseService.GetOperationCategoriesAndAutoCommentKvp();

            foreach (var operation in operations)
            {
                operation.ResolveType(operationTypes);
                operation.ResolveCategoryAndAutoComment(operationCategoriesAndAutoComment);
            }

            logger.Info($"{operations.Count(o => o.Type != "TODO")}/{operations.Count} operation types resolved");
            operations.Where(o => o.Type == "TODO").ToList().ForEach(o => logger.Debug($"Operation needs a type: '{o.Date};{o.Flow};{o.Treasury};{o.Label}'"));
            logger.Info($"{operations.Count(o => o.Category != "TODO")}/{operations.Count} operation categories resolved");
        }

        private decimal GetBankFlow(string[] splitedOperation)
        {
            if (string.IsNullOrEmpty(splitedOperation[2]))
                return decimal.Parse(splitedOperation[3]);
            else
                return decimal.Parse(splitedOperation[2]);
        }

        public void ImportPaypalFile(string paypalFilePath)
        {
            logger.Info($"Importing {paypalFilePath} paypal file");
            var csvOperations = fileSystemService.ReadAllLines(paypalFilePath);
            List<Operation> completeOperations = MatchPaypalDataToExistingOperations(csvOperations);
            bankDatabaseService.UpdateOperations(completeOperations.Select(o => o.MapToDto()).ToList());
            fileSystemService.ArchiveFile(paypalFilePath, PAYPAL_ARCHIVE_FOLDER);
        }

        private List<Operation> MatchPaypalDataToExistingOperations(List<string> csvOperations)
        {
            var operationsQueue = new Queue<PaypalOperation>(GetPaypalOperationsFromCSV(csvOperations).OrderBy(o => o.Date));
            var incompletePaypalOperationsDto = bankDatabaseService.GetUnresolvedPaypalOperations().OrderBy(o => o.Date).ToList();
            var paypalCategories = bankDatabaseService.GetPaypalCategoriesKvp();

            var completeOperations = new List<Operation>();
            while (operationsQueue.Count > 0)
            {
                var paypalOperation = operationsQueue.Dequeue();
                var operationToCompleteDto = incompletePaypalOperationsDto
                    .FirstOrDefault(o => o.Date == paypalOperation.OffesetedDate() && o.Flow == paypalOperation.Net);

                if (operationToCompleteDto == null)
                {
                    paypalOperation.DateOffset++;
                    if (paypalOperation.DateOffset > 31)
                        break;
                    operationsQueue.Enqueue(paypalOperation);
                    continue;
                }

                incompletePaypalOperationsDto.Remove(operationToCompleteDto);
                var completeOperation = Operation.Map(operationToCompleteDto);
                completeOperation.AutoComment = paypalOperation.Nom;
                completeOperation.ResolvePaypalCategory(paypalCategories);
                completeOperations.Add(completeOperation);
            }

            logger.Info(operationsQueue.Count > 0 ? $"{operationsQueue.Count} paypal operations could not be matched" : "All paypal operations were matched to data");
            while (operationsQueue.Count > 0)
            {
                var unmatchedOp = operationsQueue.Dequeue();
                logger.Debug($"Following paypal operation could not be matched to data: '{unmatchedOp.Date};{unmatchedOp.Net};{unmatchedOp.Nom}'");
            }

            return completeOperations;
        }

        private List<PaypalOperation> GetPaypalOperationsFromCSV(List<string> csvOperations)
        {
            var operations = new List<PaypalOperation>();

            foreach (var operation in csvOperations.Skip(1))
            {
                var operationFeilds = GetCSVFeilds(operation, ",");
                var paypalOperation = new PaypalOperation()
                {
                    Date = DateTime.Parse(operationFeilds[0]),
                    Net = decimal.Parse(operationFeilds[7]),
                    Nom = operationFeilds[11]
                };
                if (paypalOperation.Net < 0)
                    operations.Add(paypalOperation);
            }

            return operations;
        }

        private List<string> GetCSVFeilds(string operation, string delimiter)
        {
            var result = new List<string>();
            string lastIncompleteFeild = string.Empty;

            foreach (var feild in operation.Split(delimiter))
            {
                if (feild.StartsWith("\"") && feild.EndsWith("\""))
                {
                    result.Add(feild.Replace("\"", ""));
                }
                else if (!feild.StartsWith("\"") && feild.EndsWith("\""))
                {
                    result.Add($"{lastIncompleteFeild},{feild.Replace("\"", "")}");
                }
                else if (feild.StartsWith("\"") && !feild.EndsWith("\""))
                {
                    lastIncompleteFeild = feild.Replace("\"", "");
                }
                else
                {
                    result.Add(feild);
                }
            }

            return result;
        }

        public void RecomputeEveryOperationAdditionalData()
        {
            logger.Info("Re-computing every operation additional data");

            var operations = bankDatabaseService.GetAllOperations().Select(Operation.Map).ToList();
            var types = bankDatabaseService.GetOperationTypesKvp();
            var catAndComments = bankDatabaseService.GetOperationCategoriesAndAutoCommentKvp();
            var paypalCat = bankDatabaseService.GetPaypalCategoriesKvp();

            foreach (var operation in operations)
            {
                operation.ResolveType(types);
                if (operation.Type == "Paypal")
                    operation.ResolvePaypalCategory(paypalCat);
                else
                    operation.ResolveCategoryAndAutoComment(catAndComments);
            }

            logger.Info($"{operations.Count(o => o.Type == "TODO" || o.Category == "TODO")} operations still require manual input");

            bankDatabaseService.UpdateOperations(operations.Select(o => o.MapToDto()).ToList());
        }
    }
}
