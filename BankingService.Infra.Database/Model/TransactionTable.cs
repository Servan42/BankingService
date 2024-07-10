using BankingService.Core.SPI.DTOs;
using BankingService.Infra.Database.SPI.Interfaces;
using System.Globalization;

namespace BankingService.Infra.Database.Model
{
    internal class TransactionTable
    {
        private readonly IFileSystemServiceForFileDB fileSystemService;
        private readonly IBankDatabaseConfiguration config;

        public static string TablePath => Path.Combine("Database", "Transactions.table");
        public static string Header => "Id;Date;Flow;Treasury;Label;Type;CategoryId;AutoComment;Comment";

        public Dictionary<int, TransactionLine> Data { get; }
        private TransactionTable(Dictionary<int, TransactionLine> data, IFileSystemServiceForFileDB fileSystemService, IBankDatabaseConfiguration config)
        {
            this.Data = data;
            this.fileSystemService = fileSystemService;
            this.config = config;
        }

        public static TransactionTable Load(IFileSystemServiceForFileDB fileSystemService, IBankDatabaseConfiguration config)
        {
            var csvLines = fileSystemService.ReadAllLinesDecrypt(Path.Combine(config.DatabasePath, TablePath), config.DatabaseKey);
            return new TransactionTable(csvLines.Skip(1).ToDictionary(TransactionLine.GetIdFromCSV, TransactionLine.BuildFromCsv), fileSystemService, config);
        }

        internal void SaveAll()
        {
            List<string> transactionsToWrite = [Header];
            transactionsToWrite.AddRange(this.Data.Select(o => o.Value).OrderBy(o => o.Date).Select(o => o.GetCSV()));
            fileSystemService.WriteAllLinesOverrideEncrypt(Path.Combine(config.DatabasePath, TablePath), transactionsToWrite, this.config.DatabaseKey);
        }

        internal List<string> GetUniqueIdentifiersFromData()
        {
            return this.Data.Values
                .Select(x => x.GetUniqueIdentifier())
                .ToList();
        }

        internal int GetNextId()
        {
            if (this.Data.Count == 0)
                return 1;

            return this.Data.Keys.Max() + 1;
        }
    }

    internal class TransactionLine
    {
        public int? Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Flow { get; set; }
        public decimal Treasury { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public int CategoryId { get; set; }
        public string AutoComment { get; set; }
        public string Comment { get; set; }

        internal static int GetIdFromCSV(string csv)
        {
            return int.Parse(csv.Split(";")[0]);
        }

        internal static TransactionLine BuildFromCsv(string csv)
        {
            var splitted = csv.Split(";");
            return new TransactionLine
            {
                Id = int.Parse(splitted[0]),
                Date = DateTime.Parse(splitted[1]),
                Flow = decimal.Parse(splitted[2], CultureInfo.GetCultureInfo("fr-FR")),
                Treasury = decimal.Parse(splitted[3], CultureInfo.GetCultureInfo("fr-FR")),
                Label = splitted[4],
                Type = splitted[5],
                CategoryId = int.Parse(splitted[6]),
                AutoComment = splitted[7],
                Comment = splitted[8],
            };
        }

        [Obsolete]
        internal static TransactionLine Map(TransactionDto transactionDto, int categoryId)
        {
            return new TransactionLine
            {
                Id = transactionDto.Id,
                Date = transactionDto.Date,
                Flow = transactionDto.Flow,
                Treasury = transactionDto.Treasury,
                Label = transactionDto.Label,
                Type = transactionDto.Type,
                CategoryId = categoryId,
                AutoComment = transactionDto.AutoComment,
                Comment = transactionDto.Comment,
            };
        }

        [Obsolete]
        internal static TransactionLine Map(UpdatableTransactionDto updatableTransactionDto, int categoryId)
        {
            return new TransactionLine
            {
                Id = updatableTransactionDto.Id,
                Type = updatableTransactionDto.Type,
                CategoryId = categoryId,
                AutoComment = updatableTransactionDto.AutoComment,
                Comment = updatableTransactionDto.Comment
            };
        }

        [Obsolete]
        internal TransactionDto MapToDto(string resolvedCategory)
        {
            return new TransactionDto
            {
                Id = Id,
                Date = Date,
                Flow = Flow,
                Treasury = Treasury,
                Label = Label,
                Type = Type,
                Category = resolvedCategory,
                AutoComment = AutoComment,
                Comment = Comment
            };
        }

        internal string GetCSV()
        {
            var culture = CultureInfo.GetCultureInfo("fr-FR");
            return $"{Id};{Date:yyyy-MM-dd};{Flow.ToString("0.00", culture)};{Treasury.ToString("0.00", culture)};{Label};{Type};{CategoryId};{AutoComment};{Comment}";
        }

        internal string GetUniqueIdentifier()
        {
            var culture = CultureInfo.GetCultureInfo("fr-FR");
            return $"{Date:yyyy-MM-dd};{Flow.ToString("0.00", culture)};{Treasury.ToString("0.00", culture)};{Label}";
        }
    }
}
