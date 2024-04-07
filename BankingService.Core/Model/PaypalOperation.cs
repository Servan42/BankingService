using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Core.Model
{
    internal class PaypalOperation
    {
        public DateTime Date { get; internal set; }
        public decimal Net { get; internal set; }
        public string Nom { get; internal set; }
    }
}
