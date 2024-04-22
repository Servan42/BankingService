using BankingService.Infra.FileSystem.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Infra.FileSystem.Adapters
{
    public class FileSystemAdapterDatabase : Infra.Database.SPI.Interfaces.IFileSystemService
    {
        private readonly IFileSystemService fileSystemService;

        public FileSystemAdapterDatabase(IFileSystemService fileSystemService)
        {
            this.fileSystemService = fileSystemService;
        }

        public List<string> ReadAllLines(string filePath)
        {
            return fileSystemService.ReadAllLines(filePath);
        }

        public List<string> ReadAllLinesDecrypt(string filePath, string encryptionKey)
        {
            return fileSystemService.ReadAllLinesDecrypt(filePath, encryptionKey);
        }

        public void WriteAllLinesOverride(string filePath, List<string> lines)
        {
            fileSystemService.WriteAllLinesOverride(filePath, lines);
        }

        public void WriteAllLinesOverrideEncrypt(string filePath, List<string> lines, string encryptionKey)
        {
            fileSystemService.WriteAllLinesOverrideEncrypt(filePath, lines, encryptionKey);
        }

        public void ZipBackupFilesToFolder(List<string> filesToBackup, string backupFolder)
        {
            fileSystemService.ZipBackupFilesToFolder(filesToBackup, backupFolder);
        }
    }
}
