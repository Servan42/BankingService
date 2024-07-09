using AutoMapper;
using BankingService.Core.SPI.DTOs;
using BankingService.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
