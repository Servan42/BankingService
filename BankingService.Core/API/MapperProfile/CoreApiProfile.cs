using AutoMapper;
using BankingService.Core.API.DTOs;
using BankingService.Core.Model;

namespace BankingService.Core.API.MapperProfile
{
    public class CoreApiProfile : Profile
    {
        public CoreApiProfile()
        {
            CreateMap<Transaction, TransactionDto>();
            CreateMap<UpdatableTransactionDto, UpdatableTransaction>();
            CreateMap<TransactionReport, TransactionsReportDto>();
            CreateMap<DataTag, DataTagDto>();
            CreateMap<HighestTransaction, HighestTransactionDto>();
        }
    }
}
