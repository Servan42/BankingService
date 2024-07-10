using BankingService.Core.API.DTOs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
