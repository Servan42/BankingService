using BankingService.Core.SPI.DTOs;
using BankingService.Infra.Database.SPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Infra.Database.Model
{
    internal class Operations
    {
        private readonly IFileSystemService fileSystemService;
        private readonly string encryptionKey;

        public static string Path => "Database/Operations.table";
        public static string Header => "Date;Flow;Treasury;Label;Type;CategoryId;AutoComment;Comment";

        public Dictionary<string, Operation> Data { get; }
        private Operations(Dictionary<string, Operation> data, IFileSystemService fileSystemService, string encryptionKey)
        {
            this.Data = data;
            this.fileSystemService = fileSystemService;
            this.encryptionKey = encryptionKey;
        }

        public static Operations Load(IFileSystemService fileSystemService, string encryptionKey)
        {
            var csvLines = fileSystemService.ReadAllLinesDecrypt(Path, encryptionKey);
            return new Operations(csvLines.Skip(1).ToDictionary(Operation.GetKey, Operation.Map), fileSystemService, encryptionKey);
        }

        internal void SaveAll()
        {
            List<string> operationsToWrite = [Header];
            operationsToWrite.AddRange(this.Data.Select(o => o.Value).OrderBy(o => o.Date).Select(o => o.GetCSV()));
            fileSystemService.WriteAllLinesOverrideEncrypt(Path, operationsToWrite, this.encryptionKey);
        }
    }

    internal class Operation
    {
        public DateTime Date { get; set; }
        public decimal Flow { get; set; }
        public decimal Treasury { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public int CategoryId { get; set; }
        public string AutoComment { get; set; }
        public string Comment { get; set; }

        internal static string GetKey(string csv)
        {
            return string.Join(";", csv.Split(";").Take(4));
        }

        internal static Operation Map(OperationDto operationDto, int categoryId)
        {
            return new Operation
            {
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
                Date = DateTime.Parse(splitted[0]),
                Flow = decimal.Parse(splitted[1]),
                Treasury = decimal.Parse(splitted[2]),
                Label = splitted[3],
                Type = splitted[4],
                CategoryId = int.Parse(splitted[5]),
                AutoComment = splitted[6],
                Comment = splitted[7],
            };
        }

        internal OperationDto MapToDto(string resolvedCategory)
        {
            return new OperationDto
            {
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
            return $"{Date:yyyy-MM-dd};{Flow:0.00};{Treasury:0.00};{Label};{Type};{CategoryId};{AutoComment};{Comment}";
        }

        internal string GetKey()
        {
            return $"{Date:yyyy-MM-dd};{Flow:0.00};{Treasury:0.00};{Label}";
        }
    }
}
