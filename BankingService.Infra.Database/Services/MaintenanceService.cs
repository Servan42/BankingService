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
        private string encryptionKey;

        public MaintenanceService(IFileSystemService fileSystemService, string encryptionKey)
        {
            this.fileSystemService = fileSystemService;
            this.encryptionKey = encryptionKey;
        }

        public void ExportOperationsTable()
        {
            var csvLines = fileSystemService.ReadAllLinesDecrypt(Operations.Path, encryptionKey);
            File.WriteAllLines(Operations.Path + ".export", csvLines);
        }
    }
}
