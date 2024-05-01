using BankingService.Core.API.DTOs;
using BankingService.Core.API.Interfaces;
using BankingService.Core.Model;
using BankingService.Core.SPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Core.Services
{
    public class ReportService : IReportService
    {
        private readonly IBankDatabaseService bankDatabaseService;

        public ReportService(IBankDatabaseService bankDatabaseService)
        {
            this.bankDatabaseService = bankDatabaseService;
        }

        public OperationsReportDto GetOperationsReport(DateTime startDateIncluded, DateTime endDateIncluded, decimal highestOperationMinAmount = -100m)
        {
            var reportResult = new OperationReport(startDateIncluded, endDateIncluded);
            var operations = bankDatabaseService.GetOperationsBetweenDates(startDateIncluded, endDateIncluded);
            reportResult.SetTreasuryGraphData(operations);

            foreach (var operation in operations)
            {
                reportResult.AddToSumPerCategory(operation.Category, operation.Flow);
                reportResult.AddToBalances(operation.Category, operation.Flow);
                reportResult.AddToSums(operation.Category, operation.Flow);
                reportResult.AddHighestOperation(operation, highestOperationMinAmount);
            }

            return reportResult.MapToDto();
        }
    }
}
