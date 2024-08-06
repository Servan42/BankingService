using BankingService.Infra.Database.SPI.Interfaces;

namespace BankingService.Infra.Database.Model
{
    internal class TypeTable
    {
        public static string TablePath => Path.Combine("Database", "Types.csv");
        public static string Header => "StringToScan;AssociatedType";

        public Dictionary<string, TypeLine> Data { get; }
        private TypeTable(Dictionary<string, TypeLine> data)
        {
            this.Data = data;
        }

        public static TypeTable Load(IFileSystemServiceForFileDB fileSystemService, IBankDatabaseConfiguration config)
        {
            return new TypeTable(fileSystemService
                .ReadAllLines(Path.Combine(config.DatabasePath, TablePath))
                .Skip(1)
                .Select(l => l.Split(";"))
                .ToDictionary(s => s[0], s => new TypeLine(s[0], s[1])));
        }
    }

    internal class TypeLine
    {
        public TypeLine(string stringToScan, string associatedType)
        {
            StringToScan = stringToScan;
            AssociatedType = associatedType;
        }

        public string StringToScan { get; set; }
        public string AssociatedType { get; set; }
    }
}
