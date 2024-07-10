using BankingService.Infra.Database.SPI.Interfaces;

namespace BankingService.Infra.Database.Model
{
    internal class PaypalCategorieTable
    {
        public static string TablePath => Path.Combine("Database", "PaypalCategories.csv");
        public static string Header => "StringToScan;AssociatedCategoryId";

        public Dictionary<string, PaypalCategorieLine> Data { get; }
        private PaypalCategorieTable(Dictionary<string, PaypalCategorieLine> data)
        {
            this.Data = data;
        }

        public static PaypalCategorieTable Load(IFileSystemServiceForFileDB fileSystemService, IBankDatabaseConfiguration config)
        {
            return new PaypalCategorieTable(
                fileSystemService
                .ReadAllLines(Path.Combine(config.DatabasePath, TablePath))
                .Skip(1)
                .Select(l => l.Split(";"))
                .ToDictionary(s => s[0], s => new PaypalCategorieLine(s[0], int.Parse(s[1])))
                );
        }
    }

    internal class PaypalCategorieLine
    {
        public PaypalCategorieLine(string stringToScan, int categoryId)
        {
            StringToScan = stringToScan;
            CategoryId = categoryId;
        }

        public string StringToScan { get; set; }
        public int CategoryId { get; set; }
    }
}
