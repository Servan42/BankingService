﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Core.SPI.DTOs
{
    public record UpdatableOperationDto
    {
        public int? Id { get; init; }
        public string Type { get; init; }
        public string Category { get; init; }
        public string AutoComment { get; init; }
        public string Comment { get; init; }
    }
}
