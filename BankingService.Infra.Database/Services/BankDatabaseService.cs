using BankingService.Core.SPI.DTOs;
using BankingService.Infra.Database.Model;
using BankingService.Infra.Database.SPI.Interfaces;
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
        private const string FILE_CAT_AND_AUTOCOMMENT = "Database/CategoriesAndAutoComments.csv";
        private const string FILE_OPERATIONS = "Database/Operations.csv";
        private const string DATABASE_BACKUP_FOLDER = "Database/Backups";
        
        private readonly IFileSystemService fileSystemService;

        public BankDatabaseService(IFileSystemService fileSystemService)
        {
            this.fileSystemService = fileSystemService;
        }

        public void BackupDatabase()
        {
            this.fileSystemService.ZipBackupFilesToFolder([FILE_TYPES, FILE_CAT_AND_AUTOCOMMENT, FILE_OPERATIONS], DATABASE_BACKUP_FOLDER);
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
            var csv = fileSystemService.ReadAllLines(FILE_OPERATIONS);
            var header = csv.First();
            var storedOperations = csv.Skip(1).ToDictionary(Operation.GetKey, Operation.Map);

            foreach (var newOperation in operationsDto.Select(Operation.Map))
            {
                if (storedOperations.ContainsKey(newOperation.GetKey()))
                    continue;

                storedOperations.Add(newOperation.GetKey(), newOperation);
            }

            List<string> operationsToWrite = [header];
            operationsToWrite.AddRange(storedOperations.Select(o => o.Value).OrderBy(o => o.Date).Select(o => o.GetCSV()));
            fileSystemService.WriteAllLinesOverride(FILE_OPERATIONS, operationsToWrite);
        }

        public List<OperationDto> GetUnresolvedPaypalOperations()
        {
            return fileSystemService
                .ReadAllLines(FILE_OPERATIONS)
                .Skip(1)
                .Select(csv => Operation.Map(csv).MapToDto())
                .Where(o => o.Type == "Paypal" && o.Category == "TODO" && o.AutoComment == "")
                .ToList();
        }

        public void UpdateOperations(List<OperationDto> operationsDto)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> GetPaypalCategories()
        {
            throw new NotImplementedException();
        }
    }
}
