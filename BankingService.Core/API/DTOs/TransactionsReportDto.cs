namespace BankingService.Core.API.DTOs
{
    public record TransactionsReportDto
    {
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public Dictionary<string, decimal> SumPerCategory { get; init; }
        public decimal Balance { get; init; }
        public decimal BalanceWithoutSavings { get; init; }
        public decimal PositiveSum { get; init; }
        public decimal NegativeSum { get; init; }
        public decimal PositiveSumWithoutSavings { get; init; }
        public decimal NegativeSumWithoutSavings { get; init; }
        public List<HighestTransactionDto> HighestTransactions { get; init; }
        public List<DataTagDto> TreasuryGraphData { get; set; }
    }
}
