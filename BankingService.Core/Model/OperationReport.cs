using BankingService.Core.API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Core.Model
{
    internal class OperationReport
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

        public OperationReport(DateTime startDate, DateTime endDate)
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
            if(SumPerCategory.ContainsKey(category))
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
            if(flow > 0)
            {
                positiveSum += flow;
                if(category != SAVINGS_CATEGORY) positiveSumWithoutSavings += flow;
            }
            else
            {
                negativeSum += flow;
                if (category != SAVINGS_CATEGORY) negativeSumWithoutSavings += flow;
            }
        }

        internal OperationsReportDto MapToDto()
        {
            return new OperationsReportDto
            {
                StartDate = startDate,
                EndDate = endDate,
                SumPerCategory = SumPerCategory,
                Balance = balance,
                BalanceWithoutSavings = balanceWithoutSavings,
                NegativeSum = negativeSum,
                PositiveSum = positiveSum,
                NegativeSumWithoutSavings = negativeSumWithoutSavings,
                PositiveSumWithoutSavings = positiveSumWithoutSavings
            };
        }
    }
}
