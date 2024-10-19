using BankingService.Infra.Database.SPI.Interfaces;

namespace BankingService.Infra.Database.Model
{
    internal class CategorieTable
    {
        public static string TableName => "Categories.csv";
        public static string Header => "Id;Name";

        public Dictionary<int, CategorieLine> Data { get; }
        private CategorieTable(Dictionary<int, CategorieLine> data)
        {
            this.Data = data;
        }

        public static CategorieTable Load(IFileSystemServiceForFileDB fileSystemService, IBankDatabaseConfiguration config)
        {
            return new CategorieTable(
                fileSystemService
                .ReadAllLines(Path.Combine(config.DatabasePath, TableName))
                .Skip(1)
                .Select(l => l.Split(";"))
                .ToDictionary(s => int.Parse(s[0]), s => new CategorieLine(int.Parse(s[0]), s[1]))
                );
        }
    }

    internal class CategorieLine
    {
        public CategorieLine(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
