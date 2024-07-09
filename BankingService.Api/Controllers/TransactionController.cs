using BankingService.Core.API.DTOs;
using BankingService.Core.API.Interfaces;
using BankingService.Core.SPI.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BankingService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly ITransactionService transactionService;
        [Obsolete]
        private readonly IBankDatabaseService databaseService;

        public TransactionController(ITransactionService transactionService, IBankDatabaseService databaseService)
        {
            this.transactionService = transactionService;
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
                var transactions = this.transactionService.GetAllTransactions();
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
        [Route("GetTransactionCategoriesNames")]
        [ProducesResponseType<IEnumerable<string>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<string>> GetTransactionCategoriesNames()
        {
            try
            {
                var categories = this.transactionService.GetTransactionCategoriesNames();
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
                this.transactionService.UpdateTransactions(transactionsToUpdate);
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
