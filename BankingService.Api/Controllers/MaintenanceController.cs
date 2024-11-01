using BankingService.Infra.Database.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankingService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceController : Controller
    {
        private readonly IMaintenanceService maintenanceService;

        public MaintenanceController(IMaintenanceService maintenanceService)
        {
            this.maintenanceService = maintenanceService;
        }

        [HttpPost]
        [Route("BackupDB")]
        [ProducesResponseType<string>(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<string> BackupDB()
        {
            this.maintenanceService.BackupDatabase();
            return NoContent();
        }
    }
}
