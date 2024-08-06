namespace BankingService.Core.Model
{
    internal class UpdatableTransaction
    {
        public int? Id { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public string AutoComment { get; set; }
        public string Comment { get; set; }
    }
}
