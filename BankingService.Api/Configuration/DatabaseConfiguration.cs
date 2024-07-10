using BankingService.Infra.Database.SPI.Interfaces;

namespace BankingService.Api.Configuration
{
    internal class DatabaseConfiguration : IBankDatabaseConfiguration
    {
        public DatabaseConfiguration(IConfiguration configuration)
        {
            DatabaseKey = configuration.GetSection("Database:DatabaseKey").Value ?? "";
            DatabasePath = configuration.GetSection("Database:DatabasePath").Value ?? "";
        }

        public string DatabaseKey { get; init; }
        public string DatabasePath { get; init; }
    }
}
