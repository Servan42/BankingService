using BankingService.Infra.Database.Model;
using BankingService.Infra.Database.SPI.Interfaces;

namespace BankingService.Infra.Database.Services
{
    public class MaintenanceService
    {
        private readonly IFileSystemService fileSystemService;
        private readonly IBankDatabaseConfiguration dbConfig;

        public MaintenanceService(IFileSystemService fileSystemService, IBankDatabaseConfiguration bankDatabaseConfiguration)
        {
            this.fileSystemService = fileSystemService;
            this.dbConfig = bankDatabaseConfiguration;
        }

        public void ExportOperationsTable()
        {
            var csvLines = fileSystemService.ReadAllLinesDecrypt(Path.Combine(dbConfig.DatabasePath, Operations.TablePath), dbConfig.DatabaseKey);
            File.WriteAllLines(Path.Combine(dbConfig.DatabasePath, Operations.TablePath) + ".export", csvLines);
        }
    }
}
