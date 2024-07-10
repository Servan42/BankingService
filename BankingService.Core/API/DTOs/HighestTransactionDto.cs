namespace BankingService.Core.API.DTOs
{
    public record HighestTransactionDto
    {
        public DateTime Date { get; init; }
        public decimal Flow { get; init; }
        public string Type { get; init; }
        public string Category { get; init; }
        public string AutoComment { get; init; }
        public string Comment { get; init; }
    }
}
