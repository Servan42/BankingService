using BankingService.Infra.Database.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.ConsoleApp.Commands
{
    internal class ExportClearOperationsCommand : Command
    {
        private readonly MaintenanceService maintenanceService;

        public ExportClearOperationsCommand(MaintenanceService maintenanceService)
        {
            this.maintenanceService = maintenanceService;
        }

        public override string Name => "export";

        public override string ShortManual => "Maintenance: Exports a clear CSV of stored operations.";

        public override void Execute(string[] args)
        {
            this.maintenanceService.ExportOperationsTable();
            Console.WriteLine("Operation table exported to CSV next to exe.");
        }
    }
}
