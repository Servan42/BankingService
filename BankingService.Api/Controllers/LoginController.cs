using AutoMapper;
using BankingService.Api.Controllers.ApiDTOs;
using BankingService.Core.API.DTOs;
using BankingService.Core.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IMapper mapper;

        public LoginController(IAuthenticationService authenticationService, IMapper mapper)
        {
            this.authenticationService = authenticationService;
            this.mapper = mapper;
        }

        [HttpGet, AllowAnonymous]
        [ProducesResponseType(typeof(LoginTokenApiDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login(string username, string password)
        {
            var result = this.authenticationService.Login(username, password);
            if (!result.IsSuccess)
                return Unauthorized();
            
            return Ok(mapper.Map<LoginTokenApiDto>(result.Payload));
        }
    }
}
