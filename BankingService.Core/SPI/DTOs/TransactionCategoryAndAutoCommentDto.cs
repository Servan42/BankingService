namespace BankingService.Core.SPI.DTOs
{
    public record TransactionCategoryAndAutoCommentDto
    {
        public string Category { get; init; }
        public string AutoComment { get; init; }
    }
}
