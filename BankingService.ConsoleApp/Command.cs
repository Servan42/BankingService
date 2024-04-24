using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.ConsoleApp
{
    internal abstract class Command
    {
        public abstract string Name { get; }
        public abstract string ShortManual { get; }

        public abstract void Execute(string[] args);
    }
}
