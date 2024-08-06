using BankingService.Core.SPI.DTOs;

namespace BankingService.Core.Model
{
    internal class Transaction
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

        public UpdatableTransaction ToUpdatableTransaction()
        {
            return new UpdatableTransaction
            {
                Id = Id,
                Type = Type,
                Category = Category,
                AutoComment = AutoComment,
                Comment = Comment,
            };
        }

        internal void ResolveCategoryAndAutoComment(Dictionary<string, TransactionCategoryAndAutoComment> transactionCategoriesAndAutoComment)
        {
            var defaultCategory = "TODO";
            if(!string.IsNullOrEmpty(this.Category))
                defaultCategory = this.Category;
            this.Category = ResolveTransactionKeyValue(transactionCategoriesAndAutoComment.ToDictionary(o => o.Key, o => o.Value.Category), this.Label, defaultCategory);
            this.AutoComment = ResolveTransactionKeyValue(transactionCategoriesAndAutoComment.ToDictionary(o => o.Key, o => o.Value.AutoComment), this.Label, String.Empty);
        }

        internal void ResolveType(Dictionary<string, string> transactionTypes)
        {
            this.Type = ResolveTransactionKeyValue(transactionTypes, this.Label, "TODO");
        }

        internal void ResolvePaypalCategory(Dictionary<string, string> paypalCategories)
        {
            this.Category = ResolveTransactionKeyValue(paypalCategories, this.AutoComment, "TODO");
        }

        private string ResolveTransactionKeyValue(Dictionary<string, string> dict, string source, string defaultValue)
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
