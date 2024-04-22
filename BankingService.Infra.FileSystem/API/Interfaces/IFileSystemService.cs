using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Infra.FileSystem.API.Interfaces
{
    public interface IFileSystemService
    {
        void ArchiveFile(string filePath, string archiveFolder);
        public List<string> ReadAllLines(string filePath);
        public List<string> ReadAllLinesDecrypt(string filePath, string encryptionKey);
        public void WriteAllLinesOverride(string filePath, List<string> lines);
        public void WriteAllLinesOverrideEncrypt(string filePath, List<string> lines, string encryptionKey);

        void ZipBackupFilesToFolder(List<string> filesToBackup, string backupFolder);
    }
}
