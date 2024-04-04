using BankingService.Core.API.Interfaces;
using BankingService.Core.Model;
using BankingService.Core.SPI.DTOs;
using BankingService.Core.SPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Core.Services
{
    public class ImportService : IImportService
    {
        private readonly IFileSystemService fileSystemService;
        private readonly IBankDatabaseService bankDatabaseService;

        public ImportService(IFileSystemService fileSystemService, IBankDatabaseService bankDatabaseService)
        {
            this.fileSystemService = fileSystemService;
            this.bankDatabaseService = bankDatabaseService;
        }

        public void ImportBankFile(string bankFilePath)
        {
            fileSystemService.ArchiveFile(bankFilePath, "Archive/CM_Import");
            var csvOperations = fileSystemService.ReadAllLines(bankFilePath);
            List<Operation> operations = GetOperationsFromCSV(csvOperations);
            ResolveOperationsAutoFields(operations);
            bankDatabaseService.InsertOperationsIfNew(operations.Select(o => o.MapToDto()).ToList());
        }

        private List<Operation> GetOperationsFromCSV(List<string> csvOperations)
        {
            var operations = new List<Operation>();

            foreach (var csvOperation in csvOperations.Skip(1))
            {
                var splitedOperation = csvOperation.Split(";");
                operations.Add(new Operation
                {
                    Date = DateTime.Parse(splitedOperation[0]),
                    Flow = GetFlow(splitedOperation),
                    Label = splitedOperation[4],
                    Treasury = decimal.Parse(splitedOperation[5])
                });
            }

            return operations;
        }

        private void ResolveOperationsAutoFields(List<Operation> operations)
        {
            var operationTypes = bankDatabaseService.GetOperationTypes();
            var operationCategoriesAndAutoComment = bankDatabaseService.GetOperationCategoriesAndAutoComment();
            foreach (var operation in operations)
            {
                operation.ResolveType(operationTypes);
                operation.ResolveCategoryAndAutoComment(operationCategoriesAndAutoComment);
            }
        }

        private decimal GetFlow(string[] splitedOperation)
        {
            if (string.IsNullOrEmpty(splitedOperation[2]))
                return decimal.Parse(splitedOperation[3]);
            else
                return decimal.Parse(splitedOperation[2]);
        }
    }
}
