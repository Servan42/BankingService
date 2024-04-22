using BankingService.ConsoleApp;
using BankingService.Core.API.Interfaces;
using BankingService.Core.Services;
using BankingService.Core.SPI.Interfaces;
using BankingService.Infra.Database.Services;
using BankingService.Infra.Database.SPI.Interfaces;
using BankingService.Infra.FileSystem.Adapters;
using BankingService.Infra.FileSystem.Services;
using Microsoft.Extensions.Configuration;
using NLog;

internal class Program
{
    private static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("Hello, World!");

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var fileSystemService = new FileSystemService();
            var fileSystemServiceCore = new FileSystemAdapterCore(fileSystemService);
            var fileSystemServiceDatabase = new FileSystemAdapterDatabase(fileSystemService);
            IBankDatabaseConfiguration dbConfig = new DatabaseConfiguration(config);
            IBankDatabaseService bankDataBaseService = new BankDatabaseService(fileSystemServiceDatabase, dbConfig);
            IImportService importService = new ImportService(fileSystemServiceCore, bankDataBaseService);

            new MaintenanceService(fileSystemServiceDatabase, dbConfig).ExportOperationsTable();

            //bankDataBaseService.BackupDatabase();
            importService.RecomputeEveryOperationAdditionalData();
            importService.ImportBankFile(@"F:\Servan\Autres\Code\C#\BankCSVParser\publish\CSV\TODO\***REMOVED***_Jan.csv");
            importService.ImportPaypalFile(@"F:\Servan\Autres\Code\C#\BankCSVParser\publish\CSV\TODO\paypa_janvier.CSV");

            var uiManager = new UserInteractionManager(importService, bankDataBaseService);
            uiManager.ExecuteManualFillLoop();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex);
            Console.ResetColor();
            LogManager.GetCurrentClassLogger().Fatal(ex);
        }
    }
}