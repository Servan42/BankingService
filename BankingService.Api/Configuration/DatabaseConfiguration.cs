using BankingService.Infra.Database.SPI.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Api.Configuration
{
    internal class DatabaseConfiguration : IBankDatabaseConfiguration
    {
        public DatabaseConfiguration(IConfiguration configuration)
        {
            DatabaseKey = configuration.GetSection("Database:DatabaseKey").Value ?? "";
            DatabasePath = configuration.GetSection("Database:DatabasePath").Value ?? "";
        }

        public string DatabaseKey { get; init; }
        public string DatabasePath { get; init; }
    }
}
