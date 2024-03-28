using BankingService.Core.SPI.DTOs;
using BankingService.Infra.Database.SPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Infra.Database.API.Services
{
    public class BankDatabaseService : Core.SPI.Interfaces.IBankDatabaseService
    {
        private const string TYPES_FILE = "Database/types.csv";
        private const string CAT_AND_AUTOCOMMENT_FILE = "Database/CategoriesAndAutoComments.csv";

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
            foreach(var type in fileSystemService.ReadAllLines(TYPES_FILE).Skip(1))
            {
                var splittedLine = type.Split(";");
                result.Add(splittedLine[0], splittedLine[1]);
            }
            return result;
        }

        public void InsertOperationsIfNew(List<OperationDto> operations)
        {
            throw new NotImplementedException();
        }
    }
}
