using BankingService.Core.SPI.DTOs;
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
        private const string FILE_TYPES = "Database/Types.csv";
        private const string FILE_PAYPAL_CAT = "Database/PaypalCategories.csv";
        private const string FILE_CAT_AND_AUTOCOMMENT = "Database/CategoriesAndAutoComments.csv";
        private const string FILE_OPERATIONS = "Database/Operations.csv";
        private const string DATABASE_BACKUP_FOLDER = "Database/Backups";

        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IFileSystemService fileSystemService;

        public BankDatabaseService(IFileSystemService fileSystemService)
        {
            this.fileSystemService = fileSystemService;
        }

        public void BackupDatabase()
        {
            logger.Info("Backuping database");
            this.fileSystemService.ZipBackupFilesToFolder([FILE_TYPES, FILE_CAT_AND_AUTOCOMMENT, FILE_OPERATIONS, FILE_PAYPAL_CAT], DATABASE_BACKUP_FOLDER);
        }

        public Dictionary<string, OperationCategoryAndAutoCommentDto> GetOperationCategoriesAndAutoComment()
        {
            var result = new Dictionary<string, OperationCategoryAndAutoCommentDto>();
            foreach (var type in fileSystemService.ReadAllLines(FILE_CAT_AND_AUTOCOMMENT).Skip(1))
            {
                var splittedLine = type.Split(";");
                result.Add(splittedLine[0], new OperationCategoryAndAutoCommentDto
                {
                    Category = splittedLine[1],
                    AutoComment = splittedLine[2]
                });
            }
            return result;
        }

        public Dictionary<string, string> GetOperationTypes()
        {
            var result = new Dictionary<string, string>();
            foreach (var type in fileSystemService.ReadAllLines(FILE_TYPES).Skip(1))
            {
                var splittedLine = type.Split(";");
                result.Add(splittedLine[0], splittedLine[1]);
            }
            return result;
        }

        public void InsertOperationsIfNew(List<OperationDto> operationsDto)
        {
            (var header, var storedOperations) = GetStoredOperationsWithKey();
            int newOperationCount = 0;

            foreach (var newOperation in operationsDto.Select(Operation.Map))
            {
                if (storedOperations.ContainsKey(newOperation.GetKey()))
                {
                    logger.Debug($"Following operation will not be imported because it already exists: '{newOperation.GetKey()}'");
                    continue;
                }

                newOperationCount++;
                storedOperations.Add(newOperation.GetKey(), newOperation);
            }

            logger.Info($"{newOperationCount} new operations added to database");
            WriteOperationsToFile(header, storedOperations);
        }

        private (string header, Dictionary<string, Operation> data) GetStoredOperationsWithKey()
        {
            var csv = fileSystemService.ReadAllLines(FILE_OPERATIONS);
            var header = csv.First();
            var storedOperations = csv.Skip(1).ToDictionary(Operation.GetKey, Operation.Map);
            return (header, storedOperations);
        }

        private IEnumerable<OperationDto> GetStoredOperationsAsDtos()
        {
            return fileSystemService
                .ReadAllLines(FILE_OPERATIONS)
                .Skip(1)
                .Select(csv => Operation.Map(csv).MapToDto());
        }

        private void WriteOperationsToFile(string header, Dictionary<string, Operation> operations)
        {
            List<string> operationsToWrite = [header];
            operationsToWrite.AddRange(operations.Select(o => o.Value).OrderBy(o => o.Date).Select(o => o.GetCSV()));
            fileSystemService.WriteAllLinesOverride(FILE_OPERATIONS, operationsToWrite);
        }

        public List<OperationDto> GetUnresolvedPaypalOperations()
        {
            return GetStoredOperationsAsDtos()
                .Where(o => o.Type == "Paypal" && o.Category == "TODO" && o.AutoComment == "")
                .ToList();
        }

        public void UpdateOperations(List<OperationDto> operationsDto)
        {
            (var header, var storedOperations) = GetStoredOperationsWithKey();

            foreach(var operationToUpdate in operationsDto.Select(Operation.Map))
            {
                if (!storedOperations.ContainsKey(operationToUpdate.GetKey()))
                    throw new Exception($"Operation '{operationToUpdate.GetKey()}' cannot be updated because it is not present in database");

                var storedOperation = storedOperations[operationToUpdate.GetKey()];
                storedOperation.Type = operationToUpdate.Type;
                storedOperation.Category = operationToUpdate.Category;
                storedOperation.AutoComment = operationToUpdate.AutoComment;
                storedOperation.Comment = operationToUpdate.Comment;
            }

            logger.Info($"{operationsDto.Count} operations updated");
            WriteOperationsToFile(header, storedOperations);
        }

        public Dictionary<string, string> GetPaypalCategories()
        {
            var result = new Dictionary<string, string>();
            foreach (var type in fileSystemService.ReadAllLines(FILE_PAYPAL_CAT).Skip(1))
            {
                var splittedLine = type.Split(";");
                result.Add(splittedLine[0], splittedLine[1]);
            }
            return result;
        }

        public List<OperationDto> GetAllOperations()
        {
            return GetStoredOperationsAsDtos().ToList();
        }

        public List<OperationDto> GetOperationsThatNeedsManualInput()
        {
            return GetStoredOperationsAsDtos()
                .Where(o => o.Category == "TODO")
                .ToList();
        }
    }
}
