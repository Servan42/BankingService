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
        private Dictionary<string, decimal> SumPerCategory = new();

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

        internal OperationsReportDto MapToDto()
        {
            return new OperationsReportDto
            {
                SumPerCategory = SumPerCategory
            };
        }
    }
}
