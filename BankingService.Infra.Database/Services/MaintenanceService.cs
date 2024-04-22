using BankingService.Infra.Database.Model;
using BankingService.Infra.Database.SPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Infra.Database.Services
{
    public class MaintenanceService
    {
        private readonly IFileSystemService fileSystemService;
        private readonly IBankDatabaseConfiguration bankDatabaseConfiguration;

        public MaintenanceService(IFileSystemService fileSystemService, IBankDatabaseConfiguration bankDatabaseConfiguration)
        {
            this.fileSystemService = fileSystemService;
            this.bankDatabaseConfiguration = bankDatabaseConfiguration;
        }

        public void ExportOperationsTable()
        {
            var csvLines = fileSystemService.ReadAllLinesDecrypt(Operations.Path, bankDatabaseConfiguration.DatabaseKey);
            File.WriteAllLines(Operations.Path + ".export", csvLines);
        }
    }
}
