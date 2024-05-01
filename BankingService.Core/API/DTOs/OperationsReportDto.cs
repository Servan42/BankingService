using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Core.API.DTOs
{
    public record OperationsReportDto
    {
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public Dictionary<string, decimal> SumPerCategory { get; init; }
        public decimal Balance { get; init; }
        public decimal BalanceWithoutSavings { get; init; }
        public decimal PositiveSum { get; init; }
        public decimal NegativeSum { get; init; }
        public decimal PositiveSumWithoutSavings { get; init; }
        public decimal NegativeSumWithoutSavings { get; init; }
        public List<HighestOperationDto> HighestOperations { get; init; }
        public List<(DateTime, decimal)> TreasuryGraphData { get; set; }
    }
}
