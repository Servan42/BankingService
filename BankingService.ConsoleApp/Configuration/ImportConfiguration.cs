using BankingService.Core.SPI.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.ConsoleApp.Configuration
{
    internal class ImportConfiguration : IImportConfiguration
    {
        public ImportConfiguration(IConfiguration configuration) 
        { 
            ArchiveFolderPath = configuration.GetSection("Import:ArchiveFolderPath").Value ?? "";
        }

        public string ArchiveFolderPath { get; init; }
    }
}
