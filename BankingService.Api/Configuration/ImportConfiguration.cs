using BankingService.Core.SPI.Interfaces;

namespace BankingService.Api.Configuration
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
