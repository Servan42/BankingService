using BankingService.Core.API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Core.API.Interfaces
{
    public interface IReportService
    {
        public TransactionsReportDto GetTransactionsReport(DateTime startDateIncluded, DateTime endDateIncluded, decimal highestTransactionMinAmount = -100m);
    }
}
