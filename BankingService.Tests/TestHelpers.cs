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
        internal static bool CheckTransactionDtos(List<TransactionDto> actual, List<TransactionDto> expected)
        {
            Assert.That(actual.Count, Is.EqualTo(expected.Count), "Not the same amount of elements");
            for (int i = 0; i < actual.Count; i++)
            {
                Assert.That(actual[i], Is.EqualTo(expected[i]), $"index:{i}");
            }

            return true;
        }

        internal static bool CheckUpdatableTransactionDtos(List<UpdatableTransactionDto> actual, List<UpdatableTransactionDto> expected)
        {
            Assert.That(actual.Count, Is.EqualTo(expected.Count), "Not the same amount of elements");
            for (int i = 0; i < actual.Count; i++)
            {
                Assert.That(actual[i], Is.EqualTo(expected[i]), $"index:{i}");
            }

            return true;
        }

        internal static bool CheckStringList(List<string> transactions, List<string> expected)
        {
            Assert.That(transactions.Count, Is.EqualTo(expected.Count));
            for (int i = 0; i < transactions.Count; i++)
            {
                Assert.That(transactions[i], Is.EqualTo(expected[i]), $"index:{i}");
            }
            return true;
        }
    }
}
