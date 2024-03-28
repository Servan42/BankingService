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
        private const string TYPES_FILE = "Database/types.csv";
        private const string CAT_AND_AUTOCOMMENT_FILE = "Database/CategoriesAndAutoComments.csv";
        private const string OPERATIONS_FILE = "Database/Operations.csv";

        private readonly IFileSystemService fileSystemService;

        public BankDatabaseService(IFileSystemService fileSystemService)
        {
            this.fileSystemService = fileSystemService;
        }

        public Dictionary<string, OperationCategoryAndAutoCommentDto> GetOperationCategoriesAndAutoComment()
        {
            var result = new Dictionary<string, OperationCategoryAndAutoCommentDto>();
            foreach (var type in fileSystemService.ReadAllLines(CAT_AND_AUTOCOMMENT_FILE).Skip(1))
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
            foreach (var type in fileSystemService.ReadAllLines(TYPES_FILE).Skip(1))
            {
                var splittedLine = type.Split(";");
                result.Add(splittedLine[0], splittedLine[1]);
            }
            return result;
        }

        public void InsertOperationsIfNew(List<OperationDto> operationsDto)
        {
            var csv = fileSystemService.ReadAllLines(OPERATIONS_FILE);
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
            fileSystemService.WriteAllLinesOverride(OPERATIONS_FILE, operationsToWrite);
        }
    }
}
