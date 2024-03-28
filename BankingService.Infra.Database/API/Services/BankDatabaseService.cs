﻿using BankingService.Core.SPI.DTOs;
using BankingService.Infra.Database.SPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Infra.Database.API.Services
{
    public class BankDatabaseService : Core.SPI.Interfaces.IBankDatabaseService
    {
        private readonly IFileSystemService fileSystemService;

        public BankDatabaseService(IFileSystemService fileSystemService)
        {
            this.fileSystemService = fileSystemService;
        }

        public Dictionary<string, string> GetOperationAutoComments()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> GetOperationCategories()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> GetOperationTypes()
        {
            var result = new Dictionary<string, string>();
            foreach(var type in fileSystemService.ReadAllLines("Database/types.csv").Skip(1))
            {
                var splittedLine = type.Split(";");
                result.Add(splittedLine[0], splittedLine[1]);
            }
            return result;
        }

        public void InsertOperationsIfNew(List<OperationDto> operations)
        {
            throw new NotImplementedException();
        }
    }
}