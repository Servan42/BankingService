using BankingService.Core.SPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Core.SPI.Interfaces
{
    public interface IBankDatabaseService
    {
        public void InsertOperationsIfNew(List<OperationDto> operationsDto);
        public Dictionary<string, string> GetOperationTypes();
        public Dictionary<string, string> GetPaypalCategories();
        public Dictionary<string, OperationCategoryAndAutoCommentDto> GetOperationCategoriesAndAutoComment();
        public void BackupDatabase();
        public List<OperationDto> GetUnresolvedPaypalOperations();
        public void UpdateOperations(List<OperationDto> operationsDto);
    }
}
