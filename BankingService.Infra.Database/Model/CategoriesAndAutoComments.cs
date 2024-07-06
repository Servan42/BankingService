using BankingService.Infra.Database.SPI.Interfaces;

namespace BankingService.Infra.Database.Model
{
    internal class CategoriesAndAutoComments
    {
        public static string TablePath => Path.Combine("Database", "CategoriesAndAutoComments.csv");
        public static string Header => "StringToScan;AssociatedCategoryId;AssociatedCommentAuto";

        public Dictionary<string, TransactionCategoryAndAutoComment> Data { get; }
        private CategoriesAndAutoComments(Dictionary<string, TransactionCategoryAndAutoComment> data)
        {
            this.Data = data;
        }

        public static CategoriesAndAutoComments Load(IFileSystemService fileSystemService, IBankDatabaseConfiguration config)
        {
            return new CategoriesAndAutoComments(
                fileSystemService
                .ReadAllLines(Path.Combine(config.DatabasePath, TablePath))
                .Skip(1)
                .Select(l => l.Split(";"))
                .ToDictionary(s => s[0], s => new TransactionCategoryAndAutoComment(s[0], int.Parse(s[1]), s[2]))
                );
        }
    }

    internal class TransactionCategoryAndAutoComment
    {
        public TransactionCategoryAndAutoComment(string stringToScan, int categoryId, string autoComment)
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
