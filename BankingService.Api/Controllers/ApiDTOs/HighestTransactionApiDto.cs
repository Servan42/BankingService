namespace BankingService.Api.Controllers.ApiDTOs
{
    public record HighestTransactionApiDto
    {
        public DateTime Date { get; init; }
        public decimal Flow { get; init; }
        public string Type { get; init; }
        public string Category { get; init; }
        public string AutoComment { get; init; }
        public string Comment { get; init; }
    }
}
