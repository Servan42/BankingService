using BankingService.Core.API.Interfaces;
using BankingService.Core.SPI.DTOs;
using BankingService.Core.SPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Core.API.Services
{
    public class ImportService : IImportService
    {
        private const string BANK_FILE_HEADER = "Date;Date de valeur;Débit;Crédit;Libellé;Solde";
        private readonly IFileSystemService fileSystemService;
        private readonly IBankDatabaseService bankDatabaseService;

        public ImportService(IFileSystemService fileSystemService, IBankDatabaseService bankDatabaseService)
        {
            this.fileSystemService = fileSystemService;
            this.bankDatabaseService = bankDatabaseService;
        }

        public void ImportBankFile(string bankFilePath)
        {
            var csvOperations = this.fileSystemService.ReadAllLines(bankFilePath);
            var operations = new List<OperationDto>();

            foreach (var csvOperation in csvOperations)
            {
                if (csvOperation == BANK_FILE_HEADER)
                    continue;

                var splitedOperation = csvOperation.Split(";");
                operations.Add(new OperationDto
                {
                    Date = DateTime.Parse(splitedOperation[0]),
                    Flow = GetFlow(splitedOperation),
                    Label = splitedOperation[4],
                    Treasury = decimal.Parse(splitedOperation[5])
                });
            }

            var operationTypes = bankDatabaseService.GetOperationTypes();
            foreach (var operation in operations)
            {
                operation.Type = ResolveOperationKeyValue(operation, operationTypes);
            }

            var operationCategories = bankDatabaseService.GetOperationCategories();
            foreach (var operation in operations)
            {
                operation.Category = ResolveOperationKeyValue(operation, operationCategories);
            }

            bankDatabaseService.InsertOperationsIfNew(operations);
        }

        // TODO remove feature envy
        private string ResolveOperationKeyValue(OperationDto operation, Dictionary<string, string> dict)
        {
            foreach (var kvp in dict)
            {
                if (operation.Label.Contains(kvp.Key))
                {
                    return kvp.Value;
                }
            }
            return "TODO";
        }

        private decimal GetFlow(string[] splitedOperation)
        {
            if (string.IsNullOrEmpty(splitedOperation[2]))
                return decimal.Parse(splitedOperation[3]);
            else
                return decimal.Parse(splitedOperation[2]);
        }
    }
}
