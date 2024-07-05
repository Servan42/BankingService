using BankingService.Infra.FileSystem.API.Interfaces;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Infra.FileSystem.Services
{
    public class FileSystemService : IFileSystemService
    {
        public void ArchiveFile(string filePath, string archiveFolder)
        {
            var filenameWithoutExtention = Path.GetFileNameWithoutExtension(filePath);
            var extention = Path.GetExtension(filePath);
            var newFileName = $"{filenameWithoutExtention}-{DateTime.Now:yyyyMMdd}_{DateTime.Now:HHmmss}{extention}";
#if DEBUG
            File.Copy(filePath, Path.Combine(archiveFolder, newFileName));
#else
            File.Move(filePath, Path.Combine(archiveFolder, newFileName));
#endif
        }

        public List<string> ReadAllLines(string filePath)
        {
            return File.ReadAllLines(filePath).ToList();
        }

        public List<string> ReadAllLinesDecrypt(string filePath, string encryptionKey)
        {
            var clear = new EncryptionService().Decrypt(File.ReadAllBytes(filePath), encryptionKey);
            return clear.Split('\n').ToList();
        }

        public void WriteAllLinesOverride(string filePath, List<string> lines)
        {
            File.WriteAllLines(filePath, lines);
        }

        public void WriteAllLinesOverrideEncrypt(string filePath, List<string> lines, string encryptionKey)
        {
            File.WriteAllBytes(filePath, new EncryptionService().Encrypt(string.Join('\n', lines), encryptionKey));
        }

        public void ZipBackupFilesToFolder(List<string> filesToBackup, string backupFolder)
        {
            if (!Directory.Exists(backupFolder))
            {
                Directory.CreateDirectory(backupFolder);
            }
            if (Directory.Exists("temp"))
            {
                Directory.GetFiles("temp").ToList().ForEach(file => File.Delete(file));
                Directory.Delete("temp");
            }
            Directory.CreateDirectory("temp");
            filesToBackup.ForEach(f => File.Copy(f, Path.Combine("temp",Path.GetFileName(f))));
            var zipFileName = $"backupDB-{DateTime.Now:yyyyMMdd}_{DateTime.Now:HHmmss}.zip";
            ZipFile.CreateFromDirectory("temp", Path.Combine(backupFolder, zipFileName));
            Directory.GetFiles("temp").ToList().ForEach(File.Delete);
            Directory.Delete("temp");
        }
    }
}
