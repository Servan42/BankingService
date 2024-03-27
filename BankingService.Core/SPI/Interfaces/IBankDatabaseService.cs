using BankingService.Core.SPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Core.SPI.Interfaces
{
    public interface IBankDatabaseService
    {
        public void InsertOperationIfNew(List<OperationDto> operations);
    }
}
