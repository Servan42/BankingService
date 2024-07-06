using BankingService.Infra.Database.SPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Infra.Database.Model
{
    internal class PaypalCategories
    {
        public static string TablePath => Path.Combine("Database", "PaypalCategories.csv");
        public static string Header => "StringToScan;AssociatedCategoryId";

        public Dictionary<string, PaypalCategorie> Data { get; }
        private PaypalCategories(Dictionary<string, PaypalCategorie> data)
        {
            this.Data = data;
        }

        public static PaypalCategories Load(IFileSystemServiceForFileDB fileSystemService, IBankDatabaseConfiguration config)
        {
            return new PaypalCategories(
                fileSystemService
                .ReadAllLines(Path.Combine(config.DatabasePath, TablePath))
                .Skip(1)
                .Select(l => l.Split(";"))
                .ToDictionary(s => s[0], s => new PaypalCategorie(s[0], int.Parse(s[1])))
                );
        }
    }

    internal class PaypalCategorie
    {
        public PaypalCategorie(string stringToScan, int categoryId)
        {
            StringToScan = stringToScan;
            CategoryId = categoryId;
        }

        public string StringToScan { get; set; }
        public int CategoryId { get; set; }
    }
}
