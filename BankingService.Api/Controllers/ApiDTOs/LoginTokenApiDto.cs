namespace BankingService.Api.Controllers.ApiDTOs
{
    public record LoginTokenApiDto(string Token, DateTime ExpirationDate)
    {
    }
}
