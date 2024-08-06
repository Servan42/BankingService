namespace BankingService.Core.API.DTOs
{
    public record DataTagDto
    {
        public DateTime DateTime { get; init; }
        public decimal Value { get; init; }
    }
}
