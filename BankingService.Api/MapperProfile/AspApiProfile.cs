using AutoMapper;
using BankingService.Api.Controllers.ApiDTOs;
using BankingService.Core.API.DTOs;

namespace BankingService.Api.MapperProfile
{
    public class AspApiProfile : Profile
    {
        public AspApiProfile() 
        {
            CreateMap<TransactionsReportDto, TransactionsReportApiDto>();
            CreateMap<DataTagDto, DataTagApiDto>();
            CreateMap<HighestTransactionDto, HighestTransactionApiDto>();
        }
    }
}
