using BankingService.Core.API.DTOs;
using BankingService.SharedTechnical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Core.API.Interfaces
{
    public interface IAuthenticationService
    {
        Result<LoginTokenDto> Login(string username, string password);
    }
}
