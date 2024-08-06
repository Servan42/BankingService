namespace BankingService.Api.Controllers.ApiDTOs
{
    public record UpdatableTransactionApiDto
    {
        public int? Id { get; init; }
        public string Type { get; init; }
        public string Category { get; init; }
        public string AutoComment { get; init; }
        public string Comment { get; init; }
    }
}
