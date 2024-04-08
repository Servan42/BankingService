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
        internal static bool CheckOperationDtos(List<OperationDto> actual, List<OperationDto> expected)
        {
            Assert.That(actual.Count, Is.EqualTo(expected.Count), "Not the same amount of elements");
            for (int i = 0; i < actual.Count; i++)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(actual[i].Date, Is.EqualTo(expected[i].Date), $"index:{i}");
                    Assert.That(actual[i].Flow, Is.EqualTo(expected[i].Flow), $"index:{i}");
                    Assert.That(actual[i].Treasury, Is.EqualTo(expected[i].Treasury), $"index:{i}");
                    Assert.That(actual[i].Label, Is.EqualTo(expected[i].Label), $"index:{i}");
                    Assert.That(actual[i].Type, Is.EqualTo(expected[i].Type), $"index:{i}");
                    Assert.That(actual[i].Category, Is.EqualTo(expected[i].Category), $"index:{i}");
                    Assert.That(actual[i].AutoComment, Is.EqualTo(expected[i].AutoComment), $"index:{i}");
                    Assert.That(actual[i].Comment, Is.EqualTo(expected[i].Comment), $"index:{i}");
                });
            }

            return true;
        }

        internal static bool CheckStringList(List<string> operations, List<string> expected)
        {
            Assert.That(operations.Count, Is.EqualTo(expected.Count));
            for (int i = 0; i < operations.Count; i++)
            {
                Assert.That(operations[i], Is.EqualTo(expected[i]), $"index:{i}");
            }
            return true;
        }
    }
}
