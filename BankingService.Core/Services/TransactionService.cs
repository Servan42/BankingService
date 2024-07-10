using BankingService.Core.API.Interfaces;
using BankingService.Core.API.DTOs;
using BankingService.Core.SPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BankingService.Core.Model;

namespace BankingService.Core.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IBankDatabaseService bankDatabaseService;
        private readonly IMapper mapper;

        public TransactionService(IBankDatabaseService bankDatabaseService, IMapper mapper)
        {
            this.bankDatabaseService = bankDatabaseService;
            this.mapper = mapper;
        }

        public List<TransactionDto> GetAllTransactions()
        {
            return mapper.Map<List<TransactionDto>>(mapper.Map<List<Transaction>>(this.bankDatabaseService.GetAllTransactions()));
        }

        public void UpdateTransactions(List<UpdatableTransactionDto> transactionsToUpdate)
        {
            this.bankDatabaseService.UpdateTransactions(mapper.Map<List<SPI.DTOs.UpdatableTransactionDto>>(mapper.Map<List<UpdatableTransaction>>(transactionsToUpdate)));
        }

        public List<string> GetTransactionCategoriesNames()
        {
            return this.bankDatabaseService.GetAllCategoriesNames();
        }

        public List<string> GetTransactionTypesNames()
        {
            return this.bankDatabaseService.GetTransactionTypesKvp()
                .Values
                .Distinct()
                .ToList();
        }

        public List<TransactionDto> GetTransactionsThatNeedsManualInput()
        {
            return mapper.Map<List<TransactionDto>>(mapper.Map<List<Transaction>>(this.bankDatabaseService.GetTransactionsThatNeedsManualInput()));
        }
    }
}
