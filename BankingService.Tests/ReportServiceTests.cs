using BankingService.Core.API.DTOs;
using BankingService.Core.API.Interfaces;
using BankingService.Core.Services;
using BankingService.Core.SPI.DTOs;
using BankingService.Core.SPI.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Tests
{
    internal class ReportServiceTests
    {
        private IReportService reportService_sut;
        private Mock<IBankDatabaseService> mockBankDatabaseService;

        private DateTime startDate;
        private DateTime endDate;

        [SetUp]
        public void SetUp()
        {
            mockBankDatabaseService = new Mock<IBankDatabaseService>();
            reportService_sut = new ReportService(mockBankDatabaseService.Object);

            startDate = new DateTime(2024, 03, 26);
            endDate = new DateTime(2024, 03, 27);
            mockBankDatabaseService
                .Setup(x => x.GetOperationsBetweenDates(startDate, endDate))
                .Returns(new List<OperationDto>
                {
                    new OperationDto { Flow = -10m, Category = "C1" },
                    new OperationDto { Flow = -20m, Category = "C2" },
                    new OperationDto { Flow = -30m, Category = "C1" },
                    new OperationDto { Flow = 100m, Category = "Epargne" },
                    new OperationDto { Flow = -10m, Category = "Epargne" },
                });
        }

        [Test]
        public void Should_carry_report_dates()
        {
            // WHEN
            var result = reportService_sut.GetOperationsReport(startDate, endDate);

            // WHEN
            Assert.That(result.StartDate, Is.EqualTo(startDate));
            Assert.That(result.EndDate, Is.EqualTo(endDate));
        }

        [Test]
        public void Should_get_sum_per_categories()
        {
            // WHEN
            var result = reportService_sut.GetOperationsReport(startDate, endDate);

            // WHEN
            Assert.That(result.SumPerCategory["C1"], Is.EqualTo(-40m));
            Assert.That(result.SumPerCategory["C2"], Is.EqualTo(-20m));
            Assert.That(result.SumPerCategory["Epargne"], Is.EqualTo(90m));
        }

        [Test]
        public void Should_get_balance()
        {
            // WHEN
            var result = reportService_sut.GetOperationsReport(startDate, endDate);

            // WHEN
            Assert.That(result.Balance, Is.EqualTo(30m));
            Assert.That(result.BalanceWithoutSavings, Is.EqualTo(-60m));
        }

        [Test]
        public void Should_get_sums()
        {
            // WHEN
            var result = reportService_sut.GetOperationsReport(startDate, endDate);

            // WHEN
            Assert.That(result.PositiveSum, Is.EqualTo(100m));
            Assert.That(result.NegativeSum, Is.EqualTo(-70m));
            Assert.That(result.PositiveSumWithoutSavings, Is.EqualTo(0m));
            Assert.That(result.NegativeSumWithoutSavings, Is.EqualTo(-60m));
        }
    }
}
