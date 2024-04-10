using BankingService.Infra.Database.SPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Infra.Database.Model
{
    internal class Types
    {
        public static string Path => "Database/Types.csv";
        public static string Header => "StringToScan;AssociatedType";

        public Dictionary<string, Type> Data { get; }
        private Types(Dictionary<string, Type> data)
        {
            this.Data = data;
        }

        public static Types Load(IFileSystemService fileSystemService)
        {
            return new Types(fileSystemService
                .ReadAllLines(Path)
                .Skip(1)
                .Select(l => l.Split(";"))
                .ToDictionary(s => s[0], s => new Type(s[0], s[1])));
        }
    }

    internal class Type
    {
        public Type(string stringToScan, string associatedType)
        {
            StringToScan = stringToScan;
            AssociatedType = associatedType;
        }

        public string StringToScan { get; set; }
        public string AssociatedType { get; set; }
    }
}
