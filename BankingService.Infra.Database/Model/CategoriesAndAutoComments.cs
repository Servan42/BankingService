using BankingService.Core.SPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Infra.Database.Model
{
    internal class CategoriesAndAutoComments
    {
        public Dictionary<string, OperationCategoryAndAutoComment> Data { get; }
        private CategoriesAndAutoComments(Dictionary<string, OperationCategoryAndAutoComment> data)
        {
            this.Data = data;
        }

        public static CategoriesAndAutoComments Load(IEnumerable<string> csvLines)
        {
            return new CategoriesAndAutoComments(
                csvLines
                .Select(l => l.Split(";"))
                .ToDictionary(s => s[0], s => new OperationCategoryAndAutoComment(s[0], int.Parse(s[1]), s[2]))
                );
        }
    }

    internal class OperationCategoryAndAutoComment
    {
        public OperationCategoryAndAutoComment(string stringToScan, int categoryId, string autoComment)
        {
            StringToScan = stringToScan;
            CategoryId = categoryId;
            AutoComment = autoComment;
        }

        public string StringToScan { get; set; }
        public int CategoryId { get; set; }
        public string AutoComment { get; set; }
    }
}
