using BankingService.Core.SPI.DTOs;
using BankingService.Core.SPI.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankingService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {
        private readonly IBankDatabaseService databaseService;

        public DatabaseController(IBankDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        [HttpGet]
        [Route("GetAllOperations")]
        [ProducesResponseType<IEnumerable<OperationDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<OperationDto>> GetAllOperations()
        {
            try
            {
                var operations = this.databaseService.GetAllOperations();
                if (operations.Any())
                    return Ok(operations);
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
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
                return StatusCode(500, ex.Message);
            }
        }
    }
}
