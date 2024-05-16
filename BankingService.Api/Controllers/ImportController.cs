using BankingService.Core.API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BankingService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportController : ControllerBase
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IImportService importService;

        public ImportController(IImportService importService)
        {
            this.importService = importService;
        }

        [HttpPost]
        [Route("ImportFile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> ImportFile(IFormFile formFile, bool isBankFile)
        {
            string tempFilePath = string.Empty;
            try
            {
                if (formFile == null || formFile.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                tempFilePath = formFile.FileName.Replace(' ', '_');
                logger.Debug($"Importing file from controller (length: {formFile.Length} bytes): {Path.GetFullPath(tempFilePath)}");
                using (var fileStream = System.IO.File.Create(tempFilePath))
                {
                    await formFile.CopyToAsync(fileStream);
                }

                string result = string.Empty;
                if (isBankFile)
                    result = this.importService.ImportBankFile(tempFilePath) + " new operations imported.";
                else
                    this.importService.ImportPaypalFile(tempFilePath);

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return StatusCode(500, ex.Message);
            }
            finally
            {
                if (!string.IsNullOrEmpty(tempFilePath) && System.IO.File.Exists(tempFilePath))
                {
                    logger.Debug($"Deleting {tempFilePath}");
                    System.IO.File.Delete(tempFilePath);
                }
            }
        }
    }
}
