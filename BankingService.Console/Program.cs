using BankingService.Core.API.Interfaces;
using BankingService.Core.Services;
using BankingService.Infra.Database.Services;
using BankingService.Infra.FileSystem.Adapters;
using BankingService.Infra.FileSystem.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var fileSystemService = new FileSystemService();
        var fileSystemServiceCore = new FileSystemAdapterCore(fileSystemService);
        var fileSystemServiceDatabase = new FileSystemAdapterDatabase(fileSystemService);
        var bankDataBaseService = new BankDatabaseService(fileSystemServiceDatabase);
        IImportService importService = new ImportService(fileSystemServiceCore, bankDataBaseService);

        bankDataBaseService.BackupDatabase();
        importService.RecomputeEveryOperationAdditionalData();
        importService.ImportBankFile(@"F:\Servan\Autres\Code\C#\BankCSVParser\publish\CSV\TODO\00021505101_Jan.csv");
        importService.ImportPaypalFile(@"F:\Servan\Autres\Code\C#\BankCSVParser\publish\CSV\TODO\paypa_janvier.CSV");

        var consoleCategories = new Dictionary<string, string>();
        int i = 1;
        foreach(var cat in bankDataBaseService.GetOperationCategoriesAndAutoComment().Values.Select(cat => cat.Category).Distinct()) 
        {
            consoleCategories.Add(i.ToString(), cat);
            i++;
        }

        foreach(var operationToFill in bankDataBaseService.GetOperationsThatNeedsManualInput())
        {
            Console.WriteLine("Categories:");
            foreach (var cat in consoleCategories) Console.WriteLine($"- [{cat.Key,2}]: {cat.Value}");
            Console.WriteLine();

            Console.WriteLine("Operation to complete:");
            Console.WriteLine($"- Date: {operationToFill.Date}");
            Console.WriteLine($"- Flow: {operationToFill.Flow}");
            Console.WriteLine($"- Label: {operationToFill.Label}");
            Console.WriteLine();

            Console.Write("Enter a category number: ");
            operationToFill.Category = consoleCategories[Console.ReadLine().Trim()];
            Console.Write("Enter a comment: ");
            operationToFill.Comment = Console.ReadLine().Trim().Replace(";","_");
            Console.Clear();
            bankDataBaseService.UpdateOperations([operationToFill]);
        }
    }
}