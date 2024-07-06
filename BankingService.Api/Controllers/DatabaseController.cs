using BankingService.Core.SPI.DTOs;
using BankingService.Core.SPI.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BankingService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IBankDatabaseService databaseService;

        public DatabaseController(IBankDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        [HttpGet]
        [Route("GetAllTransactions")]
        [ProducesResponseType<IEnumerable<TransactionDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<TransactionDto>> GetAllTransactions()
        {
            try
            {
                var transactions = this.databaseService.GetAllTransactions();
                if (transactions.Any())
                    return Ok(transactions);
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAllCategoriesNames")]
        [ProducesResponseType<IEnumerable<string>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<string>> GetAllCategoriesNames()
        {
            try
            {
                var categories = this.databaseService.GetAllCategoriesNames();
                if (categories.Any())
                    return Ok(categories);
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAllTypesNames")]
        [ProducesResponseType<IEnumerable<string>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<string>> GetAllTypesNames()
        {
            try
            {
                var types = this.databaseService.GetTransactionTypesKvp().Values.Distinct();
                if (types.Any())
                    return Ok(types);
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("UpdateTransactions")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult UpdateTransactions(List<UpdatableTransactionDto> transactionsToUpdate)
        {
            try
            {
                this.databaseService.UpdateTransactions(transactionsToUpdate);
                return StatusCode(204);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
