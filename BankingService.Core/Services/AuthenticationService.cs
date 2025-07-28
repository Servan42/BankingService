using BankingService.Core.API.DTOs;
using BankingService.Core.API.Interfaces;
using BankingService.SharedTechnical;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Core.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly ILogger<AuthenticationService> logger;

        public AuthenticationService(IDateTimeProvider dateTimeProvider, ILogger<AuthenticationService> logger)
        {
            this.dateTimeProvider = dateTimeProvider;
            this.logger = logger;
        }

        public Result<LoginTokenDto> Login(string username, string password)
        {
            if (!ValidateUser(username, password))
            {
                this.logger.LogWarning("Login failed for user {username}. Invalid credentials.", username);
                return Result<LoginTokenDto>.Failure("Invalid username or password.");
            }

            var expireDate = this.dateTimeProvider.Now.AddHours(1);
            var token = GenerateToken(expireDate);

            this.logger.LogInformation("User {username} sucessfully logged in.", username);
            return Result<LoginTokenDto>.Success(new LoginTokenDto(Token: token, ExpirationDate: expireDate));
        }

        private bool ValidateUser(string username, string password)
        {
            // placeholder
            return true;
        }

        private string GenerateToken(DateTime expireDate)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("authentication_is_not_really_enabled_this_is_just_an_example"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "BankingService",
                audience: "BankingService",
                claims: new List<Claim>()
                {
                    new Claim("appId", "BankingService"),
                },
                expires: expireDate,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
