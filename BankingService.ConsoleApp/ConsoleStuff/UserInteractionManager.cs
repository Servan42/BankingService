using BankingService.Core.API.Interfaces;
using BankingService.Core.SPI.DTOs;
using BankingService.Core.SPI.Interfaces;

namespace BankingService.ConsoleApp.ConsoleStuff
{
    internal class UserInteractionManager
    {
        private readonly IImportService importService;
        private readonly IBankDatabaseService bankDataBaseService;

        public UserInteractionManager(IImportService importService, IBankDatabaseService bankDataBaseService)
        {
            this.importService = importService;
            this.bankDataBaseService = bankDataBaseService;
        }

        internal void RunMenuLoop()
        {
            while (true)
            {
                Console.WriteLine("--------------- MENU ---------------");
                Console.WriteLine("[1] Import Bank File");
                Console.WriteLine("[2] Import Paypal File");
                Console.WriteLine("[3] Manual Operation Categorization");
                Console.Write("Enter the choice: ");
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        this.PromptImportBankFile();
                        break;
                    case "2":
                        this.PromptImportPaypalFile();
                        break;
                    case "3":
                        this.ExecuteManualFillLoop();
                        break;
                }
            }
        }

        internal void ExecuteManualFillLoop()
        {
            Dictionary<string, string> consoleCategories = LoadCategoriesWithIndexes();

            var operationsToFill = bankDataBaseService.GetOperationsThatNeedsManualInput();
            int operationCount = 0;
            foreach (var operationToFill in operationsToFill)
            {
                Console.WriteLine($"--- Filling operation {++operationCount}/{operationsToFill.Count} ---\n");
                Console.WriteLine("Categories:\n");
                EnhancedConsole.DisplayStringsOnXColumns(3, 2, consoleCategories.Select(cat => $"[{cat.Key,2}]: {cat.Value}").ToList());

                DisplayOperationToFill(operationToFill);
                var (category, comment) = PromptCategoryAndComment(consoleCategories);

                var filledOperation = operationToFill with
                {
                    Category = category,
                    Comment = comment
                };

                bankDataBaseService.UpdateOperations([filledOperation]);
                Console.Clear();
            }
        }

        private (string category, string comment) PromptCategoryAndComment(Dictionary<string, string> consoleCategories)
        {
            string category;
            string comment;
            do
            {
                Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
                Console.Write("Enter a category number: ");
                category = consoleCategories[Console.ReadLine().Trim()];
                Console.Write("Enter a comment: ");
                comment = Console.ReadLine().Trim().Replace(";", "_");

                Console.WriteLine("\nAbout to update with:\n");
                Console.Write("  - Category: ");
                EnhancedConsole.WriteWithForeGroundColor(category, ConsoleColor.Green, true);
                Console.Write("  - Comment: ");
                EnhancedConsole.WriteWithForeGroundColor(comment, ConsoleColor.Green, true);
                Console.WriteLine("\nPress ENTER to save, press anything else to retry...");
            }
            while (Console.ReadKey().Key != ConsoleKey.Enter);

            return (category, comment);
        }

        private void DisplayOperationToFill(OperationDto operationToFill)
        {
            Console.WriteLine("Operation to complete:\n");
            Console.Write("  - Date: ");
            EnhancedConsole.WriteWithForeGroundColor(operationToFill.Date.ToString("d"), ConsoleColor.Cyan, true);
            Console.Write("  - Flow: ");
            EnhancedConsole.WriteWithForeGroundColor(operationToFill.Flow.ToString(), ConsoleColor.Cyan, true);
            Console.Write("  - Label: ");
            EnhancedConsole.WriteWithForeGroundColor(operationToFill.Label, ConsoleColor.Cyan, true);
            Console.Write("  - Type: ");
            EnhancedConsole.WriteWithForeGroundColor(operationToFill.Type, ConsoleColor.Cyan, true);
            Console.WriteLine();
        }

        private Dictionary<string, string> LoadCategoriesWithIndexes()
        {
            var consoleCategories = new Dictionary<string, string>();
            int i = 1;
            foreach (var cat in bankDataBaseService.GetAllCategoriesNames())
            {
                if (cat == "TODO") continue;
                consoleCategories.Add(i.ToString(), cat);
                i++;
            }
            return consoleCategories;
        }

        internal void PromptImportBankFile()
        {
            Console.Write("Enter the bank file full path: ");
            string path = Console.ReadLine();
            importService.ImportBankFile(path);
            Console.WriteLine("File imported.");
            //importService.ImportBankFile(@"F:\Servan\Autres\Code\C#\BankCSVParser\publish\CSV\TODO\***REMOVED***_Jan.csv");
        }

        internal void PromptImportPaypalFile()
        {
            Console.Write("Enter the bank file full path: ");
            string path = Console.ReadLine();
            importService.ImportPaypalFile(path);
            Console.WriteLine("File imported.");
            //importService.ImportPaypalFile(@"F:\Servan\Autres\Code\C#\BankCSVParser\publish\CSV\TODO\paypa_janvier.CSV");
        }


    }
}
