namespace BankingService.Core.Model
{
    internal class HighestTransaction
    {
        public DateTime Date { get; set; }
        public decimal Flow { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public string AutoComment { get; set; }
        public string Comment { get; set; }
    }
}
