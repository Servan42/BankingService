using BankingService.Core.SPI.DTOs;

namespace BankingService.Core.SPI.Interfaces
{
    public interface IBankDatabaseService
    {
        public int InsertOperationsIfNew(List<OperationDto> operationsDto);
        public Dictionary<string, string> GetOperationTypesKvp();
        public Dictionary<string, string> GetPaypalCategoriesKvp();
        public Dictionary<string, OperationCategoryAndAutoCommentDto> GetOperationCategoriesAndAutoCommentKvp();
        public List<OperationDto> GetUnresolvedPaypalOperations();
        public List<OperationDto> GetAllOperations();
        public void UpdateOperations(List<UpdatableOperationDto> operationsDto);
        public List<OperationDto> GetOperationsThatNeedsManualInput();
        public List<string> GetAllCategoriesNames();
        List<OperationDto> GetOperationsBetweenDates(DateTime startDateIncluded, DateTime endDateIncluded);
    }
}
