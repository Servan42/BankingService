using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Infra.Database.Model
{
    internal class Categories
    {
        public Dictionary<int, string> Data { get; }
        private Categories(Dictionary<int, string> data)
        {
            this.Data = data;
        }

        public static Categories Load(IEnumerable<string> csvLines)
        {
            return new Categories(
                csvLines
                .Select(l => l.Split(";"))
                .ToDictionary(s => int.Parse(s[0]), s => s[1])
                );
        }
    }
}
