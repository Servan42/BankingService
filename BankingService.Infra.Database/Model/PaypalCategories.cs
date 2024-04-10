using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Infra.Database.Model
{
    internal class PaypalCategories
    {
        public Dictionary<string, int> Data { get; }
        private PaypalCategories(Dictionary<string, int> data)
        {
            this.Data = data;
        }

        public static PaypalCategories Load(IEnumerable<string> csvLines)
        {
            return new PaypalCategories(
                csvLines
                .Select(l => l.Split(";"))
                .ToDictionary(s => s[0], s => int.Parse(s[1]))
                );
        }
    }
}
