using AutoMapper;
using BankingService.Core.API.DTOs;
using BankingService.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Core.API.MapperProfile
{
    public class CoreApiProfile : Profile
    {
        public CoreApiProfile()
        {
            CreateMap<Transaction, TransactionDto>();
        }
    }
}
