using BankingService.Core.API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Core.API.Interfaces
{
    public interface IImportService
    {
        public ImportReportDto ImportBankFile(string bankFilePath);
    }
}
