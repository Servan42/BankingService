namespace BankingService.Core.SPI.DTOs
{
    public record UpdatableTransactionDto
    {
        public int? Id { get; init; }
        public string Type { get; init; }
        public string Category { get; init; }
        public string AutoComment { get; init; }
        public string Comment { get; init; }
    }
}
