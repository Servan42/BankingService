using BankingService.Infra.FileSystem.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Infra.FileSystem.Adapters
{
    public class FileSystemAdapterCore : Core.SPI.Interfaces.IFileSystemService
    {
        private readonly IFileSystemService fileSystemService;

        public FileSystemAdapterCore(IFileSystemService fileSystemService)
        {
            this.fileSystemService = fileSystemService;
        }

        public void ArchiveFile(string filePath, string archiveFolder)
        {
            this.fileSystemService.ArchiveFile(filePath, archiveFolder);
        }

        public List<string> ReadAllLines(string filePath)
        {
            return fileSystemService.ReadAllLines(filePath);
        }
    }
}
