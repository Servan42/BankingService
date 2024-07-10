using AutoMapper;
using BankingService.Core.API.DTOs;
using BankingService.Core.API.Interfaces;
using BankingService.Core.Model;
using BankingService.Core.SPI.Interfaces;

namespace BankingService.Core.Services
{
    public class ReportService : IReportService
    {
        private readonly IBankDatabaseService bankDatabaseService;
        private readonly IMapper mapper;

        public ReportService(IBankDatabaseService bankDatabaseService, IMapper mapper)
        {
            this.bankDatabaseService = bankDatabaseService;
            this.mapper = mapper;
        }

        public TransactionsReportDto GetTransactionsReport(DateTime startDateIncluded, DateTime endDateIncluded, decimal highestTransactionMinAmount = -100m)
        {
            var reportResult = new TransactionReport(startDateIncluded, endDateIncluded);
            var transactions = mapper.Map<List<Transaction>>(bankDatabaseService.GetTransactionsBetweenDates(startDateIncluded, endDateIncluded));
            reportResult.SetTreasuryGraphData(transactions);

            foreach (var transaction in transactions)
            {
                reportResult.AddToSumPerCategory(transaction.Category, transaction.Flow);
                reportResult.AddToBalances(transaction.Category, transaction.Flow);
                reportResult.AddToSums(transaction.Category, transaction.Flow);
                reportResult.AddHighestTransaction(transaction, highestTransactionMinAmount);
            }

            return reportResult.MapToDto();
        }
    }
}
