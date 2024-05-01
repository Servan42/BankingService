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
        IReportService reportService_sut;
        Mock<IBankDatabaseService> mockBankDatabaseService;

        [SetUp]
        public void SetUp()
        {
            mockBankDatabaseService = new Mock<IBankDatabaseService>();
            reportService_sut = new ReportService(mockBankDatabaseService.Object);
        }

        [Test]
        public void Should_get_sum_per_categories()
        {
            // GIVEN
            var startDate = new DateTime(2024, 03, 26);
            var endDate = new DateTime(2024, 03, 27);
            mockBankDatabaseService
                .Setup(x => x.GetOperationsBetweenDates(startDate, endDate))
                .Returns(new List<OperationDto>
                {
                    new OperationDto { Flow = -10m, Category = "C1" },
                    new OperationDto { Flow = -20m, Category = "C2" },
                    new OperationDto { Flow = -30m, Category = "C1" },
                });

            // WHEN
            var result = reportService_sut.GetOperationsReport(startDate, endDate);

            // WHEN
            Assert.That(result.SumPerCategory["C1"], Is.EqualTo(-40m));
            Assert.That(result.SumPerCategory["C2"], Is.EqualTo(-20m));
        }

        [Test]
        public void Should_get_balance()
        {
            // GIVEN
            var startDate = new DateTime(2024, 03, 26);
            var endDate = new DateTime(2024, 03, 27);
            mockBankDatabaseService
                .Setup(x => x.GetOperationsBetweenDates(startDate, endDate))
                .Returns(new List<OperationDto>
                {
                    new OperationDto { Flow = -10m, Category = "C1" },
                    new OperationDto { Flow = 50m, Category = "C1" },
                    new OperationDto { Flow = -30m, Category = "C1" },
                });

            // WHEN
            var result = reportService_sut.GetOperationsReport(startDate, endDate);

            // WHEN
            Assert.That(result.Balance, Is.EqualTo(10m));
        }
    }
}
