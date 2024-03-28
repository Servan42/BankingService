using BankingService.Infra.FileSystem.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Infra.FileSystem.Services
{
    public class FileSystemService : IFileSystemService
    {
        public List<string> ReadAllLines(string filePath)
        {
            return File.ReadAllLines(filePath).ToList();
        }

        public void WriteAllLinesOverride(string filePath, List<string> lines)
        {
            File.WriteAllLines(filePath, lines);
        }
    }
}
