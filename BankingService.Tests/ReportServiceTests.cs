using AutoMapper;
using BankingService.Core.API.Interfaces;
using BankingService.Core.API.MapperProfile;
using BankingService.Core.Services;
using BankingService.Core.SPI.Interfaces;
using BankingService.Core.SPI.MapperProfile;
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
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CoreSpiProfile>();
                cfg.AddProfile<CoreApiProfile>();
            }));
            reportService_sut = new ReportService(mockBankDatabaseService.Object, mapper);

            startDate = new DateTime(2024, 03, 26);
            endDate = new DateTime(2024, 03, 27);
            mockBankDatabaseService
                .Setup(x => x.GetTransactionsBetweenDates(startDate, endDate))
                .Returns(new List<Core.SPI.DTOs.TransactionDto>
                {
                    new Core.SPI.DTOs.TransactionDto { Flow = -10m, Category = "C1", Date = new DateTime(2024, 03, 26), Treasury = 70m },
                    new Core.SPI.DTOs.TransactionDto { Flow = -20m, Category = "C2", Date = new DateTime(2024, 03, 26), Treasury = 80m, Type = "T", AutoComment = "AC", Comment= "Co" },
                    new Core.SPI.DTOs.TransactionDto { Flow = -30m, Category = "C1", Date = new DateTime(2024, 03, 27), Treasury = 130m, Type = "T2", AutoComment = "AC2", Comment= "Co2" },
                    new Core.SPI.DTOs.TransactionDto { Flow = 100m, Category = "Epargne", Date = new DateTime(2024, 03, 27), Treasury = 160m, },
                    new Core.SPI.DTOs.TransactionDto { Flow = -10m, Category = "Epargne", Date = new DateTime(2024, 03, 26), Treasury = 60m },
                });
        }

        [Test]
        public void Should_carry_report_dates()
        {
            // WHEN
            var result = reportService_sut.GetTransactionsReport(startDate, endDate);

            // WHEN
            Assert.That(result.StartDate, Is.EqualTo(startDate));
            Assert.That(result.EndDate, Is.EqualTo(endDate));
        }

        [Test]
        public void Should_get_sum_per_categories()
        {
            // WHEN
            var result = reportService_sut.GetTransactionsReport(startDate, endDate);

            // WHEN
            Assert.That(result.SumPerCategory["C1"], Is.EqualTo(-40m));
            Assert.That(result.SumPerCategory["C2"], Is.EqualTo(-20m));
            Assert.That(result.SumPerCategory["Epargne"], Is.EqualTo(90m));
        }

        [Test]
        public void Should_get_balance()
        {
            // WHEN
            var result = reportService_sut.GetTransactionsReport(startDate, endDate);

            // WHEN
            Assert.That(result.Balance, Is.EqualTo(30m));
            Assert.That(result.BalanceWithoutSavings, Is.EqualTo(-60m));
        }

        [Test]
        public void Should_get_sums()
        {
            // WHEN
            var result = reportService_sut.GetTransactionsReport(startDate, endDate);

            // WHEN
            Assert.That(result.PositiveSum, Is.EqualTo(100m));
            Assert.That(result.NegativeSum, Is.EqualTo(-70m));
            Assert.That(result.PositiveSumWithoutSavings, Is.EqualTo(0m));
            Assert.That(result.NegativeSumWithoutSavings, Is.EqualTo(-60m));
        }

        [Test]
        public void Should_get_highest_transaction()
        {
            // WHEN
            var result = reportService_sut.GetTransactionsReport(startDate, endDate, -20m);

            // WHEN
            var expectedHighestTransaction1 = new Core.API.DTOs.HighestTransactionDto
            {
                Date = new DateTime(2024, 03, 26),
                Flow = -20m,
                Type = "T",
                Category = "C2",
                AutoComment = "AC",
                Comment = "Co"
            };
            var expectedHighestTransaction2 = new Core.API.DTOs.HighestTransactionDto
            {
                Date = new DateTime(2024, 03, 27),
                Flow = -30m,
                Type = "T2",
                Category = "C1",
                AutoComment = "AC2",
                Comment = "Co2"
            };

            Assert.That(result.HighestTransactions[0], Is.EqualTo(expectedHighestTransaction1));
            Assert.That(result.HighestTransactions[1], Is.EqualTo(expectedHighestTransaction2));
        }

        [Test]
        public void Should_get_treasury_graph_data()
        {
            // WHEN
            var result = reportService_sut.GetTransactionsReport(startDate, endDate);

            // WHEN
            var expected = new List<Core.API.DTOs.DataTagDto>
            {
                new Core.API.DTOs.DataTagDto { DateTime = new DateTime(2024,03,26), Value = 80m },
                new Core.API.DTOs.DataTagDto { DateTime = new DateTime(2024,03,26), Value = 70m },
                new Core.API.DTOs.DataTagDto { DateTime = new DateTime(2024,03,26), Value = 60m },
                new Core.API.DTOs.DataTagDto { DateTime = new DateTime(2024,03,27), Value = 160m },
                new Core.API.DTOs.DataTagDto { DateTime = new DateTime(2024,03,27), Value = 130m },
            };
            CollectionAssert.AreEqual(expected, result.TreasuryGraphData);
        }
    }
}
