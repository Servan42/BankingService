using BankingService.Core.API.DTOs;
using BankingService.Core.API.Interfaces;
using Microsoft.AspNetCore.Http;
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

        public ReportController(IReportService reportService)
        {
            this.reportService = reportService;
        }

        [HttpGet]
        [ProducesResponseType<OperationsReportDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<OperationsReportDto> GetReport(DateTime startDate, DateTime endDate, int highestOperationMinAmount)
        {
            try
            {
                var report = this.reportService.GetOperationsReport(startDate, endDate, highestOperationMinAmount);
                return Ok(report);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
