using BankingService.Core.API.Interfaces;
using BankingService.Core.SPI.DTOs;
using BankingService.Core.SPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.ConsoleApp.Commands
{
    internal class ManualFillCommand : Command
    {
        private readonly IImportService importService;
        private readonly IBankDatabaseService bankDataBaseService;

        public ManualFillCommand(IImportService importService, IBankDatabaseService bankDataBaseService)
        {
            this.importService = importService;
            this.bankDataBaseService = bankDataBaseService;
        }

        public override string Name => "fill";

        public override string ShortManual => "Runs a loop that goes through the operations that needs manual categorization.";

        public override void Execute(string[] args)
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

                var filledOperation = new UpdatableOperationDto
                {
                    Id = operationToFill.Id,
                    Type = operationToFill.Type,
                    AutoComment = operationToFill.AutoComment,
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
                Console.Write("  - Comment:  ");
                EnhancedConsole.WriteWithForeGroundColor(comment, ConsoleColor.Green, true);
                Console.WriteLine("\nPress ENTER to save, press anything else to retry...");
            }
            while (Console.ReadKey().Key != ConsoleKey.Enter);

            return (category, comment);
        }

        private void DisplayOperationToFill(OperationDto operationToFill)
        {
            Console.WriteLine("Operation to complete:\n");
            Console.Write("  - Date:  ");
            EnhancedConsole.WriteWithForeGroundColor(operationToFill.Date.ToString("d"), ConsoleColor.Cyan, true);
            Console.Write("  - Flow:  ");
            EnhancedConsole.WriteWithForeGroundColor(operationToFill.Flow.ToString(), ConsoleColor.Cyan, true);
            Console.Write("  - Label: ");
            EnhancedConsole.WriteWithForeGroundColor(operationToFill.Label, ConsoleColor.Cyan, true);
            Console.Write("  - Type:  ");
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
    }
}
