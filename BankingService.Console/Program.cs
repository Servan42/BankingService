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
        importService.ImportBankFile(@"F:\Servan\Autres\Code\C#\BankCSVParser\publish\CSV\TODO\***REMOVED***_Jan.csv");
        importService.ImportPaypalFile(@"F:\Servan\Autres\Code\C#\BankCSVParser\publish\CSV\TODO\paypa_janvier.CSV");
    }
}