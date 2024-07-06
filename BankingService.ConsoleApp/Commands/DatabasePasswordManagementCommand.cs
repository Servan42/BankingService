using BankingService.Infra.Database.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.ConsoleApp.Commands
{
    internal class DatabasePasswordManagementCommand : Command
    {
        private readonly MaintenanceService maintenanceService;

        public DatabasePasswordManagementCommand(MaintenanceService maintenanceService)
        {
            this.maintenanceService = maintenanceService;
        }

        public override string Name => "dbpwd";

        public override string ShortManual => "Maintenance: Change DB password.";

        public override void Execute(string[] args)
        {
            this.maintenanceService.ChangeDatabasePassword();
        }
    }
}
