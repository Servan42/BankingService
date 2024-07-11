﻿using BankingService.Core.API.Interfaces;
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
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> ImportFile(IFormFile formFile, bool isBankFile)
        {
            string tempFilePath = string.Empty;
            try
            {
                if (formFile == null || formFile.Length == 0)
                    return BadRequest("No file uploaded.");

                if (!string.Equals(Path.GetExtension(formFile.FileName), ".csv", StringComparison.OrdinalIgnoreCase))
                    return BadRequest("Expected a CSV file.");

                tempFilePath = formFile.FileName.Replace(' ', '_');
                logger.Debug($"Importing file from controller (length: {formFile.Length} bytes): {Path.GetFullPath(tempFilePath)}");
                using (var fileStream = System.IO.File.Create(tempFilePath))
                {
                    await formFile.CopyToAsync(fileStream);
                }

                string result = string.Empty;
                if (isBankFile)
                {
                    result = this.importService.ImportBankFile(tempFilePath) + " new transactions imported.";
                    return Ok(result);
                }
                else
                {
                    this.importService.ImportPaypalFile(tempFilePath);
                    return NoContent();
                }

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
