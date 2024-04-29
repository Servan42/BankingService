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
    internal class ListIncompleteOperationsCommand : Command
    {
        private readonly IBankDatabaseService bankDataBaseService;

        public ListIncompleteOperationsCommand(IBankDatabaseService bankDataBaseService)
        {
            this.bankDataBaseService = bankDataBaseService;
        }

        public override string Name => "list";

        public override string ShortManual => "Lists the labels of the operations that do not have a category in DB.";

        public override void Execute(string[] args)
        {
            Console.WriteLine("The following operations do not have a category in DB:");
            var operations = bankDataBaseService.GetOperationsThatNeedsManualInput().OrderBy(o => o.Label);
            int paddingFlow = operations.Max(o => o.Flow.ToString().Length);

            Console.WriteLine("  Bank:");
            foreach (var operation in operations.Where(o => !o.Label.Contains("PAYPAL")))
            {
                DisplayOperation(paddingFlow, operation);
            }
            Console.WriteLine("  Paypal:");
            foreach (var operation in operations.Where(o => o.Label.Contains("PAYPAL")))
            {
                DisplayOperation(paddingFlow, operation);
            }
            Console.WriteLine($"Total: {operations.ToList().Count}");
        }

        private static void DisplayOperation(int paddingFlow, OperationDto operation)
        {
            Console.WriteLine($"    {operation.Date.ToShortDateString()}  {operation.Flow.ToString().PadLeft(paddingFlow)}  {operation.Label}");
        }
    }
}
