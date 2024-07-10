using BankingService.Infra.Database.SPI.Interfaces;

namespace BankingService.Infra.Database.Model
{
    internal class CategoriesAndAutoCommentsTable
    {
        public static string TablePath => Path.Combine("Database", "CategoriesAndAutoComments.csv");
        public static string Header => "StringToScan;AssociatedCategoryId;AssociatedCommentAuto";

        public Dictionary<string, TransactionCategoryAndAutoCommentLine> Data { get; }
        private CategoriesAndAutoCommentsTable(Dictionary<string, TransactionCategoryAndAutoCommentLine> data)
        {
            this.Data = data;
        }

        public static CategoriesAndAutoCommentsTable Load(IFileSystemServiceForFileDB fileSystemService, IBankDatabaseConfiguration config)
        {
            return new CategoriesAndAutoCommentsTable(
                fileSystemService
                .ReadAllLines(Path.Combine(config.DatabasePath, TablePath))
                .Skip(1)
                .Select(l => l.Split(";"))
                .ToDictionary(s => s[0], s => new TransactionCategoryAndAutoCommentLine(s[0], int.Parse(s[1]), s[2]))
                );
        }
    }

    internal class TransactionCategoryAndAutoCommentLine
    {
        public TransactionCategoryAndAutoCommentLine(string stringToScan, int categoryId, string autoComment)
        {
            StringToScan = stringToScan;
            CategoryId = categoryId;
            AutoComment = autoComment;
        }

        public string StringToScan { get; set; }
        public int CategoryId { get; set; }
        public string AutoComment { get; set; }
    }
}
