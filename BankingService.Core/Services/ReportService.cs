﻿using BankingService.Core.API.DTOs;
using BankingService.Core.API.Interfaces;
using BankingService.Core.Model;
using BankingService.Core.SPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Core.Services
{
    public class ReportService : IReportService
    {
        private readonly IBankDatabaseService bankDatabaseService;

        public ReportService(IBankDatabaseService bankDatabaseService)
        {
            this.bankDatabaseService = bankDatabaseService;
        }

        public OperationsReportDto GetOperationsReport(DateTime startDateIncluded, DateTime endDateIncluded)
        {
            var reportResult = new OperationReport();
            
            decimal balance = 0m;
            foreach (var operation in bankDatabaseService.GetOperationsBetweenDates(startDateIncluded, endDateIncluded))
            {
                reportResult.AddToSumPerCategory(operation.Category, operation.Flow);
                balance += operation.Flow;
            }

            reportResult.Balance = balance;
            reportResult.StartDate = startDateIncluded;
            reportResult.EndDate = endDateIncluded;

            return reportResult.MapToDto();
        }
    }
}
