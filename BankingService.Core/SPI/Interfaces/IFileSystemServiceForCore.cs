namespace BankingService.Core.SPI.Interfaces
{
    public interface IFileSystemServiceForCore
    {
        void ArchiveFile(string filePath, string archiveFolder);
        public List<string> ReadAllLines(string filePath);
    }
}
