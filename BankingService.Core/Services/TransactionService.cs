using BankingService.Core.API.Interfaces;
using BankingService.Core.SPI.DTOs;
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

        public List<API.DTOs.TransactionDto> GetAllTransactions()
        {
            return mapper.Map<List<API.DTOs.TransactionDto>>(mapper.Map<List<Transaction>>(this.bankDatabaseService.GetAllTransactions()));
        }
    }
}
