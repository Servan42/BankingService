using AutoMapper;
using BankingService.ConsoleApp;
using BankingService.ConsoleApp.Commands;
using BankingService.ConsoleApp.Configuration;
using BankingService.Core.API.Interfaces;
using BankingService.Core.API.MapperProfile;
using BankingService.Core.Services;
using BankingService.Core.SPI.Interfaces;
using BankingService.Core.SPI.MapperProfile;
using BankingService.Infra.Database.Services;
using BankingService.Infra.Database.SPI.Interfaces;
using BankingService.Infra.FileSystem.Adapters;
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

            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CoreSpiProfile>();
                cfg.AddProfile<CoreApiProfile>();
            }));

            FileSystemAdapter fileSystemAdapter = new FileSystemAdapter();
            IBankDatabaseConfiguration dbConfig = new DatabaseConfiguration(config);
            IImportConfiguration importConfig = new ImportConfiguration(config);
            IBankDatabaseService bankDataBaseService = new BankDatabaseService(fileSystemAdapter, dbConfig);
            ITransactionService transactionService = new TransactionService(bankDataBaseService, mapper);
            IReportService reportService = new ReportService(bankDataBaseService, mapper);
            IImportService importService = new ImportService(fileSystemAdapter, bankDataBaseService, mapper, importConfig);
            MaintenanceService maintenanceService = new MaintenanceService(fileSystemAdapter, dbConfig);

            Console.WriteLine("Backup database");
            maintenanceService.BackupDatabase();
            //Console.WriteLine("Recompute transactions");
            //importService.RecomputeEveryTransactionAdditionalData();

            var invoker = new CommandInvoker();
            invoker.Register(new HelpCommand(invoker));
            invoker.Register(new ImportFileCommand(importService));
            invoker.Register(new ManualFillCommand(transactionService));
            invoker.Register(new ExportClearTransactionsCommand(maintenanceService));
            invoker.Register(new BackupDbCommand(maintenanceService));
            invoker.Register(new RecomputeCategoriesCommand(importService));
            invoker.Register(new ListIncompleteTransactionsCommand(transactionService));
            invoker.Register(new ReportCommand(reportService));
            invoker.Register(new DatabaseMigrationCommand(maintenanceService));
            invoker.Register(new DatabasePasswordManagementCommand(maintenanceService));

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