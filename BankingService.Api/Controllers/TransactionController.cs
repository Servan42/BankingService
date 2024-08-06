using AutoMapper;
using BankingService.Api.Controllers.ApiDTOs;
using BankingService.Core.API.DTOs;
using BankingService.Core.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankingService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
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
            return Ok(mapper.Map<List<TransactionApiDto>>(this.transactionService.GetAllTransactions()));
        }

        [HttpGet]
        [Route("GetTransactionCategoriesNames")]
        [ProducesResponseType<IEnumerable<string>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<string>> GetTransactionCategoriesNames()
        {
            return Ok(this.transactionService.GetTransactionCategoriesNames());
        }

        [HttpGet]
        [Route("GetTransactionTypesNames")]
        [ProducesResponseType<IEnumerable<string>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<string>> GetTransactionTypesNames()
        {
            return Ok(this.transactionService.GetTransactionTypesNames());
        }

        [HttpPost]
        [Route("UpdateTransactions")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult UpdateTransactions(List<UpdatableTransactionApiDto> transactionsToUpdate)
        {
            this.transactionService.UpdateTransactions(mapper.Map<List<UpdatableTransactionDto>>(transactionsToUpdate));
            return NoContent();
        }
    }
}
