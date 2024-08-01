namespace BankingService.Core.API.Interfaces
{
    public interface IImportService
    {
        public string ImportBankFile(string bankFilePath);
        public string ImportPaypalFile(string paypalFilePath);
        public void RecomputeEveryTransactionAdditionalData();
    }
}
