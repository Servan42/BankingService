using BankingService.Core.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.ConsoleApp.Commands
{
    internal class RecomputeCategoriesCommand : Command
    {
        private readonly IImportService importService;

        public RecomputeCategoriesCommand(IImportService importService)
        {
            this.importService = importService;
        }

        public override string Name => "recompute";

        public override string ShortManual => "Recomputes the auto categories and auto comments for every operation in database.";

        public override void Execute(string[] args)
        {
            importService.RecomputeEveryOperationAdditionalData();
            Console.WriteLine("Auto categories and auto comments have been updated for every operation in database.");
        }
    }
}
