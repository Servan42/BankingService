using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Infra.Database.Model
{
    internal class Types
    {
        public Dictionary<string, string> Data { get; }
        private Types(Dictionary<string, string> data)
        {
            this.Data = data;
        }

        public static Types Load(IEnumerable<string> csvLines)
        {
            return new Types(
                csvLines
                .Select(l => l.Split(";"))
                .ToDictionary(s => s[0], s => s[1])
                );
        }
    }
}
