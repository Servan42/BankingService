using BankingService.Infra.Database.SPI.Interfaces;

namespace BankingService.Infra.Database.Model
{
    internal class CategoriesAndAutoComments
    {
        public static string Path => "Database/CategoriesAndAutoComments.csv";
        public static string Header => "StringToScan;AssociatedCategoryId;AssociatedCommentAuto";

        public Dictionary<string, OperationCategoryAndAutoComment> Data { get; }
        private CategoriesAndAutoComments(Dictionary<string, OperationCategoryAndAutoComment> data)
        {
            this.Data = data;
        }

        public static CategoriesAndAutoComments Load(IFileSystemService fileSystemService)
        {
            return new CategoriesAndAutoComments(
                fileSystemService
                .ReadAllLines(Path)
                .Skip(1)
                .Select(l => l.Split(";"))
                .ToDictionary(s => s[0], s => new OperationCategoryAndAutoComment(s[0], int.Parse(s[1]), s[2]))
                );
        }
    }

    internal class OperationCategoryAndAutoComment
    {
        public OperationCategoryAndAutoComment(string stringToScan, int categoryId, string autoComment)
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
