using AutoMapper;
using BankingService.Core.SPI.DTOs;
using BankingService.Core.Model;

namespace BankingService.Core.SPI.MapperProfile
{
    public class CoreSpiProfile : Profile
    {
        public CoreSpiProfile()
        {
            CreateMap<TransactionDto, Transaction>();
            CreateMap<UpdatableTransaction, UpdatableTransactionDto>();
        }
    }
}
