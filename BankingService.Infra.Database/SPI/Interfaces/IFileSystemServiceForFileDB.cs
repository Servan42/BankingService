using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Infra.Database.SPI.Interfaces
{
    public interface IFileSystemServiceForFileDB
    {
        public List<string> ReadAllLines(string filePath);
        public List<string> ReadAllLinesDecrypt(string filePath, string encryptionKey);
        public void WriteAllLinesOverride(string filePath, List<string> lines);
        public void WriteAllLinesOverrideEncrypt(string filePath, List<string> lines, string encryptionKey);
        void ZipBackupFilesToFolder(List<string> filesToBackup, string backupFolder);
    }
}
