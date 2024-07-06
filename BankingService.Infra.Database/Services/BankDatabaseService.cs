using BankingService.Core.SPI.DTOs;
using BankingService.Infra.Database.Model;
using BankingService.Infra.Database.SPI.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Infra.Database.Services
{
    public class BankDatabaseService : Core.SPI.Interfaces.IBankDatabaseService
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IFileSystemService fileSystemService;
        private readonly IBankDatabaseConfiguration dbConfig;

        public BankDatabaseService(IFileSystemService fileSystemService, IBankDatabaseConfiguration bankDatabaseConfiguration)
        {
            this.fileSystemService = fileSystemService;
            this.dbConfig = bankDatabaseConfiguration;
        }

        public Dictionary<string, TransactionCategoryAndAutoCommentDto> GetTransactionCategoriesAndAutoCommentKvp()
        {
            return CategoriesAndAutoComments.Load(this.fileSystemService, this.dbConfig).Data
                .Join(Categories.Load(this.fileSystemService, this.dbConfig).Data, ca => ca.Value.CategoryId, c => c.Key, (ca, c) => new {ca, c})
                .ToDictionary(j => j.ca.Key, j => new TransactionCategoryAndAutoCommentDto
                {
                    AutoComment = j.ca.Value.AutoComment,
                    Category = j.c.Value.Name
                });
        }

        public Dictionary<string, string> GetTransactionTypesKvp()
        {
            return Types.Load(this.fileSystemService, this.dbConfig).Data.ToDictionary(t => t.Key, t => t.Value.AssociatedType);
        }

        public int InsertTransactionsIfNew(List<TransactionDto> transactionsDto)
        {
            int newTransactionCount = 0;
            var transactions = Transactions.Load(this.fileSystemService, this.dbConfig);
            var existingTransactionsKeys = transactions.GetUniqueIdentifiersFromData();

            foreach (var newTransaction in ResolveTransactionDtoCategoryId(transactionsDto))
            {
                if (existingTransactionsKeys.Contains(newTransaction.GetUniqueIdentifier()))
                {
                    logger.Debug($"Following transaction will not be imported because it already exists: '{newTransaction.GetUniqueIdentifier()}'");
                    continue;
                }

                newTransactionCount++;
                newTransaction.Id = transactions.GetNextId();
                transactions.Data.Add(newTransaction.Id.Value, newTransaction);
            }

            logger.Info($"{newTransactionCount} new transactions added to database");
            transactions.SaveAll();
            return newTransactionCount;
        }

        public void UpdateTransactions(List<UpdatableTransactionDto> transactionsDto)
        {
            var storedTransactions = Transactions.Load(this.fileSystemService, this.dbConfig);

            foreach(var transactionToUpdate in ResolveUpdatebleTransactionDtoCategoryId(transactionsDto))
            {
                if (!transactionToUpdate.Id.HasValue)
                    throw new Exception($"Transaction '{transactionToUpdate.GetUniqueIdentifier()}' cannot be updated because it does not have an Id");

                if (!storedTransactions.Data.ContainsKey(transactionToUpdate.Id.Value))
                    throw new Exception($"Transaction '{transactionToUpdate.Id.Value}' cannot be updated because it is not present in database");

                var storedTransaction = storedTransactions.Data[transactionToUpdate.Id.Value];
                storedTransaction.Type = transactionToUpdate.Type;
                storedTransaction.CategoryId = transactionToUpdate.CategoryId;
                storedTransaction.AutoComment = transactionToUpdate.AutoComment;
                storedTransaction.Comment = transactionToUpdate.Comment;
            }

            logger.Info($"{transactionsDto.Count} transactions updated");
            storedTransactions.SaveAll();
        }

        private IEnumerable<Transaction> ResolveTransactionDtoCategoryId(List<TransactionDto> transactionsDto)
        {
            return transactionsDto.Join(Categories.Load(this.fileSystemService, this.dbConfig).Data, dto => dto.Category, c => c.Value.Name, (dto, c) => Transaction.Map(dto, c.Key));
        }

        private IEnumerable<Transaction> ResolveUpdatebleTransactionDtoCategoryId(List<UpdatableTransactionDto> updatableTransactionsDto)
        {
            return updatableTransactionsDto.Join(Categories.Load(this.fileSystemService, this.dbConfig).Data, dto => dto.Category, c => c.Value.Name, (dto, c) => Transaction.Map(dto, c.Key));
        }

        public Dictionary<string, string> GetPaypalCategoriesKvp()
        {
            return PaypalCategories.Load(this.fileSystemService, this.dbConfig).Data
                .Join(Categories.Load(this.fileSystemService, this.dbConfig).Data, pc => pc.Value.CategoryId, c => c.Key, (pc, c) => new { pc.Key, c.Value })
                .ToDictionary(j => j.Key, j => j.Value.Name);
        }

        public List<string> GetAllCategoriesNames()
        {
            return Categories.Load(this.fileSystemService, this.dbConfig).Data
                .Select(c => c.Value.Name)
                .ToList();
        }

        public List<TransactionDto> GetUnresolvedPaypalTransactions()
        {
            return GetStoredTransactionsAsDtos()
                .Where(o => o.Type == "Paypal" && o.Category == "TODO" && o.AutoComment == "")
                .ToList();
        }

        public List<TransactionDto> GetAllTransactions()
        {
            return GetStoredTransactionsAsDtos()
                .ToList();
        }

        public List<TransactionDto> GetTransactionsThatNeedsManualInput()
        {
            return GetStoredTransactionsAsDtos()
                .Where(o => o.Category == "TODO")
                .ToList();
        }

        private IEnumerable<TransactionDto> GetStoredTransactionsAsDtos()
        {
            return Transactions.Load(this.fileSystemService, this.dbConfig).Data
                .Join(Categories.Load(this.fileSystemService, this.dbConfig).Data, op => op.Value.CategoryId, c => c.Key, (op, ca) => op.Value.MapToDto(ca.Value.Name));
        }

        public List<TransactionDto> GetTransactionsBetweenDates(DateTime startDateIncluded, DateTime endDateIncluded)
        {
            return GetStoredTransactionsAsDtos()
                .Where(o => o.Date >= startDateIncluded && o.Date <= endDateIncluded)
                .ToList();
        }
    }
}
