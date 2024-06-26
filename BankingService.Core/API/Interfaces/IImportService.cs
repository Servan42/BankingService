﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Core.API.Interfaces
{
    public interface IImportService
    {
        public void ImportBankFile(string bankFilePath);
        public void ImportPaypalFile(string paypalFilePath);
        public void RecomputeEveryOperationAdditionalData();
    }
}
