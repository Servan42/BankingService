using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Infra.Database.API.Interfaces
{
    public interface IMaintenanceService
    {
        public void BackupDatabase();
    }
}
