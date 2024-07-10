using BankingService.Infra.Database.Services;
namespace BankingService.ConsoleApp.Commands
{
    internal class ExportClearTransactionsCommand : Command
    {
        private readonly MaintenanceService maintenanceService;

        public ExportClearTransactionsCommand(MaintenanceService maintenanceService)
        {
            this.maintenanceService = maintenanceService;
        }

        public override string Name => "export";

        public override string ShortManual => "Maintenance: Exports a clear CSV of stored transactions.";

        public override void Execute(string[] args)
        {
            this.maintenanceService.ExportTransactionsTable();
            Console.WriteLine("Transaction table exported to CSV next to exe.");
        }
    }
}
