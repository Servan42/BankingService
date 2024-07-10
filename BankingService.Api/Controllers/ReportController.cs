using AutoMapper;
using BankingService.Api.Controllers.ApiDTOs;
using BankingService.Core.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BankingService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IReportService reportService;
        private readonly IMapper mapper;

        public ReportController(IReportService reportService, IMapper mapper)
        {
            this.reportService = reportService;
            this.mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType<TransactionsReportApiDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<TransactionsReportApiDto> GetReport(DateTime startDate, DateTime endDate, int highestTransactionMinAmount)
        {
            try
            {
                var report = this.reportService.GetTransactionsReport(startDate, endDate, highestTransactionMinAmount);
                return Ok(mapper.Map<TransactionsReportApiDto>(report));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
