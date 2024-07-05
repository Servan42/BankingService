using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Infra.FileSystem.Exceptions
{
    internal class EncryptionException : Exception
    {
        public EncryptionException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
