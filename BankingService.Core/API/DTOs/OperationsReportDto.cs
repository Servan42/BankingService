using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Core.API.DTOs
{
    public record OperationsReportDto
    {
        public Dictionary<string, decimal> SumPerCategory { get; init; }
        public decimal Balance { get; init; }
    }
}
