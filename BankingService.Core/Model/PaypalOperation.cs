﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Core.Model
{
    internal class PaypalOperation
    {
        public DateTime Date { get; internal set; }
        public decimal Net { get; internal set; }
        public string Nom { get; internal set; }
        public int DateOffset { get; internal set; } = 0;

        internal DateTime OffesetedDate()
        {
            return this.Date.AddDays(this.DateOffset);
        }
    }
}
