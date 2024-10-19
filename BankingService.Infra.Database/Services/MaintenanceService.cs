using BankingService.Infra.Database.Model;
using BankingService.Infra.Database.SPI.Interfaces;
using NLog;

namespace BankingService.Infra.Database.Services
{
    public class MaintenanceService
    {
        private readonly IFileSystemServiceForFileDB fileSystemService;
        private readonly IBankDatabaseConfiguration dbConfig;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public MaintenanceService(IFileSystemServiceForFileDB fileSystemService, IBankDatabaseConfiguration bankDatabaseConfiguration)
        {
            this.fileSystemService = fileSystemService;
            this.dbConfig = bankDatabaseConfiguration;
        }

        public void ExportTransactionsTable()
        {
            logger.Info("Export transactions table to CSV");

            var csvLines = fileSystemService.ReadAllLinesDecrypt(Path.Combine(dbConfig.DatabasePath, TransactionTable.TableName), dbConfig.DatabaseKey);
            File.WriteAllLines("transactions.export", csvLines);
        }

        public void BackupDatabase()
        {
            logger.Info("Backuping database");
            this.fileSystemService.ZipBackupFilesToFolder(
                [
                    Path.Combine(dbConfig.DatabasePath, TypeTable.TableName), 
                    Path.Combine(dbConfig.DatabasePath, CategoriesAndAutoCommentsTable.TableName), 
                    Path.Combine(dbConfig.DatabasePath, TransactionTable.TableName), 
                    Path.Combine(dbConfig.DatabasePath, PaypalCategorieTable.TableName), 
                    Path.Combine(dbConfig.DatabasePath, CategorieTable.TableName)
                ], Path.Combine(dbConfig.DatabasePath, "Backups"));
        }

        public void TransactionTableMigrationToIdVersion()
        {
            logger.Info("TransactionTableMigrationToIdVersion");
            List<string> csvLines = fileSystemService.ReadAllLinesDecrypt(Path.Combine(dbConfig.DatabasePath, TransactionTable.TableName), dbConfig.DatabaseKey);

            if (csvLines.Count < 2)
                throw new Exception("No data to migrate");

            if (csvLines[0].StartsWith("Id;"))
                throw new Exception("This database was already migrated");

            int id = 1;
            List<string> transactionsToWrite = [TransactionTable.Header];
            
            foreach(var transaction in csvLines.Skip(1))
            {
                transactionsToWrite.Add(id + ";" + transaction);
                id++;
            }

            File.WriteAllLines("transactions.export.migrated", transactionsToWrite);
            Console.WriteLine("Review the file transactions.export.migrated before pressing enter to migrate the actual database. Kill the program if any anomaly is found");
            Console.ReadLine();
            fileSystemService.WriteAllLinesOverrideEncrypt(Path.Combine(dbConfig.DatabasePath, TransactionTable.TableName), transactionsToWrite, dbConfig.DatabaseKey);
            Console.WriteLine("DB MIGRATED");
            logger.Info("DB MIGRATED");
        }

        public void ChangeDatabasePassword()
        {
            Console.WriteLine($"Changing password for database {dbConfig.DatabasePath}");
            Console.Write("Enter old password (clear): ");
            string oldPassword = Console.ReadLine() ?? "";
            Console.Write("Enter new password (clear): ");
            string? newPassword = Console.ReadLine();
            if (string.IsNullOrEmpty(newPassword))
                throw new InvalidOperationException("New password cannot be empty");

            Console.Write($"Changing password from \"{oldPassword}\" to \"{newPassword}\". Kill the program if it is incorrect. Press enter to continue.");
            _ = Console.ReadLine();

            var tablePath = Path.Combine(dbConfig.DatabasePath, TransactionTable.TableName);
            var backupTablePath = $"{tablePath}.backup_{DateTime.Now.Ticks}";
            File.Copy(tablePath, backupTablePath);
            Console.WriteLine($"{backupTablePath} created");
            List<string> csvLines = fileSystemService.ReadAllLinesDecrypt(tablePath, oldPassword);
            fileSystemService.WriteAllLinesOverrideEncrypt(tablePath, csvLines, newPassword);
            Console.WriteLine("DB passcord modified successfully");
        }
    }
}
