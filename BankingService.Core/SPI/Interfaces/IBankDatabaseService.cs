using BankingService.Core.SPI.DTOs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Core.SPI.Interfaces
{
    public interface IBankDatabaseService
    {
        public void InsertOperationsIfNew(List<OperationDto> operationsDto);
        public Dictionary<string, string> GetOperationTypesKvp();
        public Dictionary<string, string> GetPaypalCategoriesKvp();
        public Dictionary<string, OperationCategoryAndAutoCommentDto> GetOperationCategoriesAndAutoCommentKvp();
        public List<OperationDto> GetUnresolvedPaypalOperations();
        public List<OperationDto> GetAllOperations();
        public void UpdateOperations(List<OperationDto> operationsDto);
        public List<OperationDto> GetOperationsThatNeedsManualInput();
        public List<string> GetAllCategoriesNames();
        List<OperationDto> GetOperationsBetweenDates(DateTime startDateIncluded, DateTime endDateIncluded);
    }
}
