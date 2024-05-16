using BankingService.Core.SPI.DTOs;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Core.Model
{
    internal class Operation
    {
        public int? Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Flow { get; set; }
        public decimal Treasury { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public string AutoComment { get; set; }
        public string Comment { get; set; }

        internal static Operation Map(OperationDto dto)
        {
            return new Operation
            {
                Id = dto.Id,
                Date = dto.Date,
                Flow = dto.Flow,
                Treasury = dto.Treasury,
                Label = dto.Label,
                Type = dto.Type,
                Category = dto.Category,
                AutoComment = dto.AutoComment,
                Comment = dto.Comment,
            };
        }

        public OperationDto MapToDto()
        {
            return new OperationDto
            {
                Id = Id,
                Date = Date,
                Flow = Flow,
                Treasury = Treasury,
                Label = Label,
                Type = Type,
                Category = Category,
                AutoComment = AutoComment,
                Comment = Comment,
            };
        }

        public UpdatableOperationDto MapToUpdatableOperationDto()
        {
            return new UpdatableOperationDto
            {
                Id = Id,
                Type = Type,
                Category = Category,
                AutoComment = AutoComment,
                Comment = Comment,
            };
        }

        internal void ResolveCategoryAndAutoComment(Dictionary<string, OperationCategoryAndAutoCommentDto> operationCategoriesAndAutoComment)
        {
            var defaultCategory = "TODO";
            if(!string.IsNullOrEmpty(this.Category))
                defaultCategory = this.Category;
            this.Category = ResolveOperationKeyValue(operationCategoriesAndAutoComment.ToDictionary(o => o.Key, o => o.Value.Category), this.Label, defaultCategory);
            this.AutoComment = ResolveOperationKeyValue(operationCategoriesAndAutoComment.ToDictionary(o => o.Key, o => o.Value.AutoComment), this.Label, String.Empty);
        }

        internal void ResolveType(Dictionary<string, string> operationTypes)
        {
            this.Type = ResolveOperationKeyValue(operationTypes, this.Label, "TODO");
        }

        internal void ResolvePaypalCategory(Dictionary<string, string> paypalCategories)
        {
            this.Category = ResolveOperationKeyValue(paypalCategories, this.AutoComment, "TODO");
        }

        private string ResolveOperationKeyValue(Dictionary<string, string> dict, string source, string defaultValue)
        {
            if (string.IsNullOrEmpty(source))
                return defaultValue;

            foreach (var kvp in dict)
            {
                if (source.Contains(kvp.Key))
                {
                    return kvp.Value;
                }
            }

            return defaultValue;
        }
    }
}
