using BankingService.Core.API.Interfaces;

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

        public override string ShortManual => "Recomputes the auto categories and auto comments for every transaction in database.";

        public override void Execute(string[] args)
        {
            importService.RecomputeEveryTransactionAdditionalData();
            Console.WriteLine("Auto categories and auto comments have been updated for every transaction in database.");
        }
    }
}
