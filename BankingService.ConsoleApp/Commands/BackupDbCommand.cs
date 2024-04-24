using BankingService.Core.SPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.ConsoleApp.Commands
{
    internal class BackupDbCommand : Command
    {
        private readonly IBankDatabaseService bankDataBaseService;

        public BackupDbCommand(IBankDatabaseService bankDataBaseService)
        {
            this.bankDataBaseService = bankDataBaseService;
        }

        public override string Name => "backup";

        public override string ShortManual => "Maintenance: Creates a backup of the database, at the configured location.";

        public override void Execute(string[] args)
        {
            this.bankDataBaseService.BackupDatabase();
            Console.WriteLine("Database backed up at the configured location.");
        }
    }
}
