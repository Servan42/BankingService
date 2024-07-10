namespace BankingService.Core.Model
{
    internal class TransactionReport
    {
        private const string SAVINGS_CATEGORY = "Epargne";

        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public decimal Balance { get; private set; }
        public decimal BalanceWithoutSavings { get; private set; }
        public decimal NegativeSum { get; private set; }
        public decimal PositiveSum { get; private set; }
        public decimal NegativeSumWithoutSavings { get; private set; }
        public decimal PositiveSumWithoutSavings { get; private set; }
        public Dictionary<string, decimal> SumPerCategory { get; private set; }
        public List<HighestTransaction> HighestTransactions { get; private set; }
        public List<DataTag> TreasuryGraphData { get; private set; }

        public TransactionReport(DateTime startDate, DateTime endDate)
        {
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.SumPerCategory = new();
            this.HighestTransactions = new();
            this.TreasuryGraphData = new();
        }

        internal void AddToBalances(string category, decimal flow)
        {
            Balance += flow;
            if (category != SAVINGS_CATEGORY) BalanceWithoutSavings += flow;
        }

        internal void AddToSumPerCategory(string category, decimal flow)
        {
            if (SumPerCategory.ContainsKey(category))
            {
                SumPerCategory[category] += flow;
            }
            else
            {
                SumPerCategory.Add(category, flow);
            }
        }

        internal void AddToSums(string category, decimal flow)
        {
            if (flow > 0)
            {
                PositiveSum += flow;
                if (category != SAVINGS_CATEGORY) PositiveSumWithoutSavings += flow;
            }
            else
            {
                NegativeSum += flow;
                if (category != SAVINGS_CATEGORY) NegativeSumWithoutSavings += flow;
            }
        }

        internal void AddHighestTransaction(Transaction transaction, decimal highestTransactionMinAmount)
        {
            if (transaction.Flow <= highestTransactionMinAmount && transaction.Category != SAVINGS_CATEGORY)
            {
                HighestTransactions.Add(new HighestTransaction
                {
                    Date = transaction.Date,
                    Flow = transaction.Flow,
                    Type = transaction.Type,
                    Category = transaction.Category,
                    AutoComment = transaction.AutoComment,
                    Comment = transaction.Comment
                });
            }
        }

        internal void SetTreasuryGraphData(List<Transaction> transactions)
        {
            TreasuryGraphData = transactions
                .OrderBy(o => o.Date)
                .ThenByDescending(o => o.Treasury)
                .Select(o => new DataTag
                {
                   DateTime = o.Date,
                   Value = o.Treasury
                })
                .ToList();
        }
    }
}
