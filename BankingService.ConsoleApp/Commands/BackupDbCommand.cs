using BankingService.Infra.Database.Services;

namespace BankingService.ConsoleApp.Commands
{
    internal class BackupDbCommand : Command
    {
        private readonly MaintenanceService maintenanceService;

        public BackupDbCommand(MaintenanceService maintenanceService)
        {
            this.maintenanceService = maintenanceService;
        }

        public override string Name => "backup";

        public override string ShortManual => "Maintenance: Creates a backup of the database, at the configured location.";

        public override void Execute(string[] args)
        {
            this.maintenanceService.BackupDatabase();
            Console.WriteLine("Database backed up at the configured location.");
        }
    }
}
