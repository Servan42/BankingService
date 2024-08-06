namespace BankingService.Infra.FileSystem.Exceptions
{
    internal class EncryptionException : Exception
    {
        public EncryptionException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
