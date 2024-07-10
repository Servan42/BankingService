using BankingService.Core.API.DTOs;

namespace BankingService.Core.API.Interfaces
{
    public interface ITransactionService
    {
        List<TransactionDto> GetAllTransactions();
        List<string> GetTransactionCategoriesNames();
        List<TransactionDto> GetTransactionsThatNeedsManualInput();
        List<string> GetTransactionTypesNames();
        void UpdateTransactions(List<UpdatableTransactionDto> transactionsToUpdate);
    }
}
