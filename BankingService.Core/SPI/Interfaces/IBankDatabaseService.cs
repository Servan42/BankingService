using BankingService.Core.SPI.DTOs;

namespace BankingService.Core.SPI.Interfaces
{
    public interface IBankDatabaseService
    {
        public int InsertTransactionsIfNew(List<TransactionDto> transactionsDto);
        public Dictionary<string, string> GetTransactionTypesKvp();
        public Dictionary<string, string> GetPaypalCategoriesKvp();
        public Dictionary<string, TransactionCategoryAndAutoCommentDto> GetTransactionCategoriesAndAutoCommentKvp();
        public List<TransactionDto> GetUnresolvedPaypalTransactions();
        public List<TransactionDto> GetAllTransactions();
        public void UpdateTransactions(List<UpdatableTransactionDto> transactionsDto);
        public List<TransactionDto> GetTransactionsThatNeedsManualInput();
        public List<string> GetAllCategoriesNames();
        List<TransactionDto> GetTransactionsBetweenDates(DateTime startDateIncluded, DateTime endDateIncluded);
    }
}
