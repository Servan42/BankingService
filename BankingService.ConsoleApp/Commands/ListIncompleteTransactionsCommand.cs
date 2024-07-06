using BankingService.Core.API.Interfaces;
using BankingService.Core.SPI.DTOs;
using BankingService.Core.SPI.Interfaces;
using NLog.LayoutRenderers.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.ConsoleApp.Commands
{
    internal class ListIncompleteTransactionsCommand : Command
    {
        private readonly IBankDatabaseService bankDataBaseService;

        public ListIncompleteTransactionsCommand(IBankDatabaseService bankDataBaseService)
        {
            this.bankDataBaseService = bankDataBaseService;
        }

        public override string Name => "list";

        public override string ShortManual => "Lists the labels of the transactions that do not have a category in DB.";

        public override void Execute(string[] args)
        {
            Console.WriteLine("The following transactions do not have a category in DB:");
            var transactions = bankDataBaseService.GetTransactionsThatNeedsManualInput().OrderBy(o => o.Label);
            int paddingFlow = transactions.Max(o => o.Flow.ToString().Length);

            Console.WriteLine("  Bank:");
            foreach (var transaction in transactions.Where(o => !o.Label.Contains("PAYPAL")))
            {
                DisplayTransaction(paddingFlow, transaction);
            }
            Console.WriteLine("  Paypal:");
            foreach (var transaction in transactions.Where(o => o.Label.Contains("PAYPAL")))
            {
                DisplayTransaction(paddingFlow, transaction);
            }
            Console.WriteLine($"Total: {transactions.ToList().Count}");
        }

        private static void DisplayTransaction(int paddingFlow, TransactionDto transaction)
        {
            Console.WriteLine($"    {transaction.Date.ToShortDateString()}  {transaction.Flow.ToString().PadLeft(paddingFlow)}  {transaction.Label}");
        }
    }
}
