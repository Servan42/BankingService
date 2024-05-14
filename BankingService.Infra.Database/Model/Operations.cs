using BankingService.Core.SPI.DTOs;
using BankingService.Infra.Database.SPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Infra.Database.Model
{
    internal class Operations
    {
        private readonly IFileSystemService fileSystemService;
        private readonly IBankDatabaseConfiguration config;

        public static string TablePath => Path.Combine("Database", "Operations.table");
        public static string Header => "Id;Date;Flow;Treasury;Label;Type;CategoryId;AutoComment;Comment";

        public Dictionary<int, Operation> Data { get; }
        private Operations(Dictionary<int, Operation> data, IFileSystemService fileSystemService, IBankDatabaseConfiguration config)
        {
            this.Data = data;
            this.fileSystemService = fileSystemService;
            this.config = config;
        }

        public static Operations Load(IFileSystemService fileSystemService, IBankDatabaseConfiguration config)
        {
            var csvLines = fileSystemService.ReadAllLinesDecrypt(Path.Combine(config.DatabasePath, TablePath), config.DatabaseKey);
            return new Operations(csvLines.Skip(1).ToDictionary(Operation.GetIdFromCSV, Operation.Map), fileSystemService, config);
        }

        internal void SaveAll()
        {
            List<string> operationsToWrite = [Header];
            operationsToWrite.AddRange(this.Data.Select(o => o.Value).OrderBy(o => o.Date).Select(o => o.GetCSV()));
            fileSystemService.WriteAllLinesOverrideEncrypt(Path.Combine(config.DatabasePath, TablePath), operationsToWrite, this.config.DatabaseKey);
        }

        internal List<string> GetUniqueIdentifiersFromData()
        {
            return this.Data.Values
                .Select(x => x.GetUniqueIdentifier())
                .ToList();
        }

        internal int GetMaxId()
        {
            return this.Data.Keys.Max();
        }
    }

    internal class Operation
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

        internal static Operation Map(OperationDto operationDto, int categoryId)
        {
            return new Operation
            {
                Id = operationDto.Id,
                Date = operationDto.Date,
                Flow = operationDto.Flow,
                Treasury = operationDto.Treasury,
                Label = operationDto.Label,
                Type = operationDto.Type,
                CategoryId = categoryId,
                AutoComment = operationDto.AutoComment,
                Comment = operationDto.Comment,
            };
        }

        internal static Operation Map(string csv)
        {
            var splitted = csv.Split(";");
            return new Operation
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

        internal OperationDto MapToDto(string resolvedCategory)
        {
            return new OperationDto
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
