namespace BankingService.Api.Controllers.ApiDTOs
{
    public record TransactionApiDto
    {
        public int? Id { get; init; }
        public DateTime Date { get; init; }
        public decimal Flow { get; init; }
        public decimal Treasury { get; init; }
        public string Label { get; init; }
        public string Type { get; init; }
        public string Category { get; init; }
        public string AutoComment { get; init; }
        public string Comment { get; init; }
    }
}
