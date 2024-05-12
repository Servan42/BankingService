using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Core.API.DTOs
{
    public record DataTagDto
    {
        public DateTime DateTime { get; init; }
        public decimal Value { get; init; }
    }
}
