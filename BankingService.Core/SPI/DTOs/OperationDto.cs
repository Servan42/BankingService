using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Core.SPI.DTOs
{
    public class OperationDto
    {
        public DateTime Date { get; set; }
        public decimal Flow { get; set; }
        public decimal Treasury { get; set; }
        public string Type { get; set; }
        public string Comment { get; set; }
        public string AutoComment { get; set; }
        public string Category { get; set; }
        public string Label { get; set; }
    }
}
