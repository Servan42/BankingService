using BankingService.Core.SPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Tests.ImportServiceTests
{
    internal static class ImportTestHelpers
    {
        internal static bool CheckOperation(List<OperationDto> actual, List<OperationDto> expected)
        {
            Assert.That(actual.Count, Is.EqualTo(expected.Count), "Not the same amount of elements");
            for (int i = 0; i < actual.Count; i++)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(actual[i].Date, Is.EqualTo(expected[i].Date));
                    Assert.That(actual[i].Flow, Is.EqualTo(expected[i].Flow));
                    Assert.That(actual[i].Treasury, Is.EqualTo(expected[i].Treasury));
                    Assert.That(actual[i].Type, Is.EqualTo(expected[i].Type));
                    Assert.That(actual[i].Comment, Is.EqualTo(expected[i].Comment));
                    Assert.That(actual[i].AutoComment, Is.EqualTo(expected[i].AutoComment));
                    Assert.That(actual[i].Category, Is.EqualTo(expected[i].Category));
                    Assert.That(actual[i].Label, Is.EqualTo(expected[i].Label));
                });
            }

            return true;
        }
    }
}
