using AutoMapper;
using BankingService.Api.Controllers.ApiDTOs;
using BankingService.Core.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankingService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService reportService;
        private readonly IMapper mapper;

        public ReportController(IReportService reportService, IMapper mapper)
        {
            this.reportService = reportService;
            this.mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType<TransactionsReportApiDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<TransactionsReportApiDto> GetReport(DateTime startDate, DateTime endDate, int highestTransactionMinAmount)
        {
            var report = this.reportService.GetTransactionsReport(startDate, endDate, highestTransactionMinAmount);
            return Ok(mapper.Map<TransactionsReportApiDto>(report));
        }
    }
}
