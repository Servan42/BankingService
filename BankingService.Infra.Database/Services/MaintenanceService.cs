using BankingService.Infra.Database.Model;
using BankingService.Infra.Database.SPI.Interfaces;
using NLog;

namespace BankingService.Infra.Database.Services
{
    public class MaintenanceService
    {
        private readonly IFileSystemService fileSystemService;
        private readonly IBankDatabaseConfiguration dbConfig;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public MaintenanceService(IFileSystemService fileSystemService, IBankDatabaseConfiguration bankDatabaseConfiguration)
        {
            this.fileSystemService = fileSystemService;
            this.dbConfig = bankDatabaseConfiguration;
        }

        public void ExportOperationsTable()
        {
            logger.Info("Export operations table to CSV");

            var csvLines = fileSystemService.ReadAllLinesDecrypt(Path.Combine(dbConfig.DatabasePath, Operations.TablePath), dbConfig.DatabaseKey);
            File.WriteAllLines("operations.export", csvLines);
        }

        public void BackupDatabase()
        {
            logger.Info("Backuping database");
            this.fileSystemService.ZipBackupFilesToFolder(
                [
                    Path.Combine(dbConfig.DatabasePath, Types.TablePath), 
                    Path.Combine(dbConfig.DatabasePath, CategoriesAndAutoComments.TablePath), 
                    Path.Combine(dbConfig.DatabasePath, Operations.TablePath), 
                    Path.Combine(dbConfig.DatabasePath, PaypalCategories.TablePath), 
                    Path.Combine(dbConfig.DatabasePath, Categories.TablePath)
                ], Path.Combine(dbConfig.DatabasePath, "Database", "Backups"));
        }
    }
}
