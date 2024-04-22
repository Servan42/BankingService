namespace BankingService.Infra.Database.SPI.Interfaces
{
    public interface IBankDatabaseConfiguration
    {
        public string DatabaseKey { get; init; }
        public string DatabasePath { get; init; }
    }
}
