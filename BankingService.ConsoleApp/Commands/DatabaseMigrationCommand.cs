using BankingService.Infra.Database.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.ConsoleApp.Commands
{
    internal class DatabaseMigrationCommand : Command
    {
        private readonly MaintenanceService maintenanceService;

        public DatabaseMigrationCommand(MaintenanceService maintenanceService)
        {
            this.maintenanceService = maintenanceService;
        }

        public override string Name => "dbmigration";

        public override string ShortManual => "Migrates the DB operation table to add IDs to it.";

        public override void Execute(string[] args)
        {
            this.maintenanceService.OperationTableMigrationToIdVersion();
        }
    }
}
