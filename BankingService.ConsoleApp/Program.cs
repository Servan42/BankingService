using BankingService.ConsoleApp;
using BankingService.ConsoleApp.Commands;
using BankingService.ConsoleApp.Configuration;
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
        ILogger logger = LogManager.GetCurrentClassLogger();

        try
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var fileSystemService = new FileSystemService();
            var fileSystemServiceCore = new FileSystemAdapterCore(fileSystemService);
            var fileSystemServiceDatabase = new FileSystemAdapterDatabase(fileSystemService);
            IBankDatabaseConfiguration dbConfig = new DatabaseConfiguration(config);
            IBankDatabaseService bankDataBaseService = new BankDatabaseService(fileSystemServiceDatabase, dbConfig);
            IReportService reportService = new ReportService(bankDataBaseService);
            IImportService importService = new ImportService(fileSystemServiceCore, bankDataBaseService);
            MaintenanceService maintenanceService = new MaintenanceService(fileSystemServiceDatabase, dbConfig);

            Console.WriteLine("Backup database");
            maintenanceService.BackupDatabase();
            //Console.WriteLine("Recompute operations");
            //importService.RecomputeEveryOperationAdditionalData();

            var invoker = new CommandInvoker();
            invoker.Register(new HelpCommand(invoker));
            invoker.Register(new ImportFileCommand(importService));
            invoker.Register(new ManualFillCommand(importService, bankDataBaseService));
            invoker.Register(new ExportClearOperationsCommand(maintenanceService));
            invoker.Register(new BackupDbCommand(maintenanceService));
            invoker.Register(new RecomputeCategoriesCommand(importService));
            invoker.Register(new ListIncompleteOperationsCommand(bankDataBaseService));
            invoker.Register(new ReportCommand(reportService));
            invoker.Register(new DatabaseMigrationCommand(maintenanceService));

            Console.WriteLine("Welcome to BankingService CLI. Type 'help' for more info.\n");
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                    continue;

                var splittedInput = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var inputCommand = splittedInput[0];
                string[] inputArgs = Array.Empty<string>();
                if (splittedInput.Length > 1)
                    inputArgs = splittedInput.Skip(1).ToArray();

                try
                {
                    invoker.Execute(inputCommand, inputArgs);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: " + ex.Message);
                    Console.ResetColor();
                    logger.Error(ex);
                }
                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex);
            Console.ResetColor();
            logger.Fatal(ex);
        }
    }
}