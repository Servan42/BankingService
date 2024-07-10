using BankingService.Core.API.DTOs;
using BankingService.Core.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.ConsoleApp.Commands
{
    internal class ManualFillCommand : Command
    {
        private readonly ITransactionService transactionService;

        public ManualFillCommand(ITransactionService transactionService)
        {
            this.transactionService = transactionService;
        }

        public override string Name => "fill";

        public override string ShortManual => "Runs a loop that goes through the transactions that needs manual categorization.";

        public override void Execute(string[] args)
        {
            Dictionary<string, string> consoleCategories = LoadCategoriesWithIndexes();

            var transactionsToFill = transactionService.GetTransactionsThatNeedsManualInput();
            int transactionCount = 0;
            foreach (var transactionToFill in transactionsToFill)
            {
                Console.WriteLine($"--- Filling transaction {++transactionCount}/{transactionsToFill.Count} ---\n");
                Console.WriteLine("Categories:\n");
                EnhancedConsole.DisplayStringsOnXColumns(3, 2, consoleCategories.Select(cat => $"[{cat.Key,2}]: {cat.Value}").ToList());

                DisplayTransactionToFill(transactionToFill);
                var (category, comment) = PromptCategoryAndComment(consoleCategories);

                var filledTransaction = new UpdatableTransactionDto
                {
                    Id = transactionToFill.Id,
                    Type = transactionToFill.Type,
                    AutoComment = transactionToFill.AutoComment,
                    Category = category,
                    Comment = comment
                };

                transactionService.UpdateTransactions([filledTransaction]);
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

        private void DisplayTransactionToFill(TransactionDto transactionToFill)
        {
            Console.WriteLine("Transaction to complete:\n");
            Console.Write("  - Date:  ");
            EnhancedConsole.WriteWithForeGroundColor(transactionToFill.Date.ToString("d"), ConsoleColor.Cyan, true);
            Console.Write("  - Flow:  ");
            EnhancedConsole.WriteWithForeGroundColor(transactionToFill.Flow.ToString(), ConsoleColor.Cyan, true);
            Console.Write("  - Label: ");
            EnhancedConsole.WriteWithForeGroundColor(transactionToFill.Label, ConsoleColor.Cyan, true);
            Console.Write("  - Type:  ");
            EnhancedConsole.WriteWithForeGroundColor(transactionToFill.Type, ConsoleColor.Cyan, true);
            Console.WriteLine();
        }

        private Dictionary<string, string> LoadCategoriesWithIndexes()
        {
            var consoleCategories = new Dictionary<string, string>();
            int i = 1;
            foreach (var cat in transactionService.GetTransactionCategoriesNames())
            {
                if (cat == "TODO") continue;
                consoleCategories.Add(i.ToString(), cat);
                i++;
            }
            return consoleCategories;
        }
    }
}
