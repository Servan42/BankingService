using BankingService.Core.API.DTOs;

namespace BankingService.Core.Model
{
    internal class TransactionReport
    {
        private const string SAVINGS_CATEGORY = "Epargne";

        private DateTime startDate;
        private DateTime endDate;
        private Dictionary<string, decimal> SumPerCategory = new();
        private decimal balance = 0;
        private decimal balanceWithoutSavings = 0;
        private decimal negativeSum;
        private decimal positiveSum;
        private decimal negativeSumWithoutSavings;
        private decimal positiveSumWithoutSavings;

        [Obsolete]
        private List<HighestTransactionDto> highestTransactions = new();
        [Obsolete]
        private List<DataTagDto> treasuryGraphData;

        public TransactionReport(DateTime startDate, DateTime endDate)
        {
            this.startDate = startDate;
            this.endDate = endDate;
        }

        internal void AddToBalances(string category, decimal flow)
        {
            balance += flow;
            if (category != SAVINGS_CATEGORY) balanceWithoutSavings += flow;
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
                positiveSum += flow;
                if (category != SAVINGS_CATEGORY) positiveSumWithoutSavings += flow;
            }
            else
            {
                negativeSum += flow;
                if (category != SAVINGS_CATEGORY) negativeSumWithoutSavings += flow;
            }
        }

        internal void AddHighestTransaction(Transaction transaction, decimal highestTransactionMinAmount)
        {
            if (transaction.Flow <= highestTransactionMinAmount && transaction.Category != SAVINGS_CATEGORY)
            {
                highestTransactions.Add(new HighestTransactionDto
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
            treasuryGraphData = transactions
                .OrderBy(o => o.Date)
                .ThenByDescending(o => o.Treasury)
                .Select(o => new DataTagDto
                {
                   DateTime = o.Date,
                   Value = o.Treasury
                })
                .ToList();
        }

        [Obsolete]
        internal TransactionsReportDto MapToDto()
        {
            return new TransactionsReportDto
            {
                StartDate = startDate,
                EndDate = endDate,
                SumPerCategory = SumPerCategory,
                Balance = balance,
                BalanceWithoutSavings = balanceWithoutSavings,
                NegativeSum = negativeSum,
                PositiveSum = positiveSum,
                NegativeSumWithoutSavings = negativeSumWithoutSavings,
                PositiveSumWithoutSavings = positiveSumWithoutSavings,
                HighestTransactions = highestTransactions,
                TreasuryGraphData = treasuryGraphData
            };
        }
    }
}
