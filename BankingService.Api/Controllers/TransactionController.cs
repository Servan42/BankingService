using AutoMapper;
using BankingService.Api.Controllers.ApiDTOs;
using BankingService.Core.API.DTOs;
using BankingService.Core.API.Interfaces;
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
        private readonly IMapper mapper;

        public TransactionController(ITransactionService transactionService, IMapper mapper)
        {
            this.transactionService = transactionService;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("GetAllTransactions")]
        [ProducesResponseType<IEnumerable<TransactionApiDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<TransactionApiDto>> GetAllTransactions()
        {
            try
            {
                var transactions = this.transactionService.GetAllTransactions();
                if (transactions.Any())
                    return Ok(mapper.Map<List<TransactionApiDto>>(transactions));
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
        [Route("GetTransactionTypesNames")]
        [ProducesResponseType<IEnumerable<string>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<string>> GetTransactionTypesNames()
        {
            try
            {
                var types = this.transactionService.GetTransactionTypesNames();
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
        public ActionResult UpdateTransactions(List<UpdatableTransactionApiDto> transactionsToUpdate)
        {
            try
            {
                this.transactionService.UpdateTransactions(mapper.Map<List<UpdatableTransactionDto>>(transactionsToUpdate));
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
