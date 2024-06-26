﻿using BankingService.Core.SPI.DTOs;
using BankingService.Infra.Database.Model;
using BankingService.Infra.Database.SPI.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Infra.Database.Services
{
    public class BankDatabaseService : Core.SPI.Interfaces.IBankDatabaseService
    {
        private readonly string DATABASE_BACKUP_FOLDER;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IFileSystemService fileSystemService;
        private readonly IBankDatabaseConfiguration dbConfig;

        public BankDatabaseService(IFileSystemService fileSystemService, IBankDatabaseConfiguration bankDatabaseConfiguration)
        {
            this.fileSystemService = fileSystemService;
            this.dbConfig = bankDatabaseConfiguration;
        }

        public Dictionary<string, OperationCategoryAndAutoCommentDto> GetOperationCategoriesAndAutoCommentKvp()
        {
            return CategoriesAndAutoComments.Load(this.fileSystemService, this.dbConfig).Data
                .Join(Categories.Load(this.fileSystemService, this.dbConfig).Data, ca => ca.Value.CategoryId, c => c.Key, (ca, c) => new {ca, c})
                .ToDictionary(j => j.ca.Key, j => new OperationCategoryAndAutoCommentDto
                {
                    AutoComment = j.ca.Value.AutoComment,
                    Category = j.c.Value.Name
                });
        }

        public Dictionary<string, string> GetOperationTypesKvp()
        {
            return Types.Load(this.fileSystemService, this.dbConfig).Data.ToDictionary(t => t.Key, t => t.Value.AssociatedType);
        }

        public void InsertOperationsIfNew(List<OperationDto> operationsDto)
        {
            int newOperationCount = 0;
            var operations = Operations.Load(this.fileSystemService, this.dbConfig);

            foreach (var newOperation in ResolveOperationDto(operationsDto))
            {
                if (operations.Data.ContainsKey(newOperation.GetKey()))
                {
                    logger.Debug($"Following operation will not be imported because it already exists: '{newOperation.GetKey()}'");
                    continue;
                }

                newOperationCount++;
                operations.Data.Add(newOperation.GetKey(), newOperation);
            }

            logger.Info($"{newOperationCount} new operations added to database");
            operations.SaveAll();
        }

        public void UpdateOperations(List<OperationDto> operationsDto)
        {
            var storedOperations = Operations.Load(this.fileSystemService, this.dbConfig);

            foreach(var operationToUpdate in ResolveOperationDto(operationsDto))
            {
                if (!storedOperations.Data.ContainsKey(operationToUpdate.GetKey()))
                    throw new Exception($"Operation '{operationToUpdate.GetKey()}' cannot be updated because it is not present in database");

                var storedOperation = storedOperations.Data[operationToUpdate.GetKey()];
                storedOperation.Type = operationToUpdate.Type;
                storedOperation.CategoryId = operationToUpdate.CategoryId;
                storedOperation.AutoComment = operationToUpdate.AutoComment;
                storedOperation.Comment = operationToUpdate.Comment;
            }

            logger.Info($"{operationsDto.Count} operations updated");
            storedOperations.SaveAll();
        }

        private IEnumerable<Operation> ResolveOperationDto(List<OperationDto> operationsDto)
        {
            return operationsDto.Join(Categories.Load(this.fileSystemService, this.dbConfig).Data, dto => dto.Category, c => c.Value.Name, (dto, c) => Operation.Map(dto, c.Key));
        }

        public Dictionary<string, string> GetPaypalCategoriesKvp()
        {
            return PaypalCategories.Load(this.fileSystemService, this.dbConfig).Data
                .Join(Categories.Load(this.fileSystemService, this.dbConfig).Data, pc => pc.Value.CategoryId, c => c.Key, (pc, c) => new { pc.Key, c.Value })
                .ToDictionary(j => j.Key, j => j.Value.Name);
        }

        public List<string> GetAllCategoriesNames()
        {
            return Categories.Load(this.fileSystemService, this.dbConfig).Data
                .Select(c => c.Value.Name)
                .ToList();
        }

        public List<OperationDto> GetUnresolvedPaypalOperations()
        {
            return GetStoredOperationsAsDtos()
                .Where(o => o.Type == "Paypal" && o.Category == "TODO" && o.AutoComment == "")
                .ToList();
        }

        public List<OperationDto> GetAllOperations()
        {
            return GetStoredOperationsAsDtos()
                .ToList();
        }

        public List<OperationDto> GetOperationsThatNeedsManualInput()
        {
            return GetStoredOperationsAsDtos()
                .Where(o => o.Category == "TODO")
                .ToList();
        }

        private IEnumerable<OperationDto> GetStoredOperationsAsDtos()
        {
            return Operations.Load(this.fileSystemService, this.dbConfig).Data
                .Join(Categories.Load(this.fileSystemService, this.dbConfig).Data, op => op.Value.CategoryId, c => c.Key, (op, ca) => op.Value.MapToDto(ca.Value.Name));
        }

        public List<OperationDto> GetOperationsBetweenDates(DateTime startDateIncluded, DateTime endDateIncluded)
        {
            return GetStoredOperationsAsDtos()
                .Where(o => o.Date >= startDateIncluded && o.Date <= endDateIncluded)
                .ToList();
        }
    }
}
