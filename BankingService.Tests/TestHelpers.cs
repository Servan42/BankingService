using BankingService.Core.SPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Tests
{
    internal static class TestHelpers
    {
        internal static bool IsEqualTo<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
        {
            CollectionAssert.AreEqual(expected, actual);
            return true;
        }
    }
}
