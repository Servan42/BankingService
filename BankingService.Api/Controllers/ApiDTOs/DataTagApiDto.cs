namespace BankingService.Api.Controllers.ApiDTOs
{
    public record DataTagApiDto
    {
        public DateTime DateTime { get; init; }
        public decimal Value { get; init; }
    }
}
