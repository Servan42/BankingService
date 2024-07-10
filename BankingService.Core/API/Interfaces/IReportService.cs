using BankingService.Core.API.DTOs;

namespace BankingService.Core.API.Interfaces
{
    public interface IReportService
    {
        public TransactionsReportDto GetTransactionsReport(DateTime startDateIncluded, DateTime endDateIncluded, decimal highestTransactionMinAmount = -100m);
    }
}
