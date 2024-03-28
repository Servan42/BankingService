using BankingService.Core.SPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Infra.Database.Model
{
    internal class Operation
    {
        public DateTime Date { get; set; }
        public decimal Flow { get; set; }
        public decimal Treasury { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public string AutoComment { get; set; }
        public string Comment { get; set; }

        internal static string GetKey(string csv)
        {
            return string.Join(";", csv.Split(";").Take(4));
        }

        internal static Operation Map(OperationDto newOperation)
        {
            return new Operation
            {
                Date = newOperation.Date,
                Flow = newOperation.Flow,
                Treasury = newOperation.Treasury,
                Label = newOperation.Label,
                Type = newOperation.Type,
                Category = newOperation.Category,
                AutoComment = newOperation.AutoComment,
                Comment = newOperation.Comment,
            };
        }

        internal static Operation Map(string csv)
        {
            var splitted = csv.Split(";");
            return new Operation
            {
                Date = DateTime.Parse(splitted[0]),
                Flow = decimal.Parse(splitted[1]),
                Treasury = decimal.Parse(splitted[2]),
                Label = splitted[3],
                Type = splitted[4],
                Category = splitted[5],
                AutoComment = splitted[6],
                Comment = splitted[7],
            };
        }

        internal string GetCSV()
        {
            return $"{Date:yyyy-MM-dd};{Flow:0.00};{Treasury:0.00};{Label};{Type};{Category};{AutoComment};{Comment}";
        }

        internal string GetKey()
        {
            return $"{Date:yyyy-MM-dd};{Flow:0.00};{Treasury:0.00};{Label}";
        }
    }
}
