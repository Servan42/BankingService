namespace BankingService.Core.API.Interfaces
{
    public interface IImportService
    {
        public string ImportBankFile(string bankFilePath);
        public void ImportPaypalFile(string paypalFilePath);
        public void RecomputeEveryTransactionAdditionalData();
    }
}
