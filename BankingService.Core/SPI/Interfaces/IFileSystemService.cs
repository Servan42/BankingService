using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Core.SPI.Interfaces
{
    public interface IFileSystemService
    {
        public List<string> ReadAllLines(string filePath);
    }
}
