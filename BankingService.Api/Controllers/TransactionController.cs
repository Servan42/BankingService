using AutoMapper;
using BankingService.Api.Controllers.ApiDTOs;
using BankingService.Core.API.DTOs;
using BankingService.Core.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System.Collections.Generic;

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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<TransactionApiDto>> GetAllTransactions()
        {
            try
            {
                return Ok(mapper.Map<List<TransactionApiDto>>(this.transactionService.GetAllTransactions()));
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<string>> GetTransactionCategoriesNames()
        {
            try
            {
                return Ok(this.transactionService.GetTransactionCategoriesNames());
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<string>> GetTransactionTypesNames()
        {
            try
            {
                return Ok(this.transactionService.GetTransactionTypesNames());
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
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
