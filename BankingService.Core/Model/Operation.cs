using BankingService.Core.SPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Core.Model
{
    internal class Operation
    {
        public DateTime Date { get; set; }
        public decimal Flow { get; set; }
        public decimal Treasury { get; set; }
        public string Type { get; set; }
        public string Comment { get; set; }
        public string AutoComment { get; set; }
        public string Category { get; set; }
        public string Label { get; set; }

        public OperationDto MapToDto()
        {
            return new OperationDto
            {
                Date = Date,
                Flow = Flow,
                Treasury = Treasury,
                AutoComment = AutoComment,
                Category = Category,
                Comment = Comment,
                Label = Label,
                Type = Type
            };
        }

        internal void ResolveCategoryAndAutoComment(Dictionary<string, OperationCategoryAndAutoCommentDto> operationCategoriesAndAutoComment)
        {
            this.Category = ResolveOperationKeyValue(operationCategoriesAndAutoComment.ToDictionary(o => o.Key, o => o.Value.Category));
            this.AutoComment = ResolveOperationKeyValue(operationCategoriesAndAutoComment.ToDictionary(o => o.Key, o => o.Value.AutoComment));
        }

        internal void ResolveType(Dictionary<string, string> operationTypes)
        {
            this.Type = ResolveOperationKeyValue(operationTypes);
        }

        private string ResolveOperationKeyValue(Dictionary<string, string> dict)
        {
            foreach (var kvp in dict)
            {
                if (this.Label.Contains(kvp.Key))
                {
                    return kvp.Value;
                }
            }
            return "TODO";
        }
    }
}
