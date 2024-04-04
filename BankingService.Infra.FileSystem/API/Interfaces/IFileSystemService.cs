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
        public void WriteAllLinesOverride(string filePath, List<string> lines);
        void ZipBackupFilesToFolder(List<string> filesToBackup, string backupFolder);
    }
}
