using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Core.SPI.DTOs
{
    public record TransactionCategoryAndAutoCommentDto
    {
        public string Category { get; init; }
        public string AutoComment { get; init; }
    }
}
