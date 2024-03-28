using BankingService.Core.SPI.DTOs;
using BankingService.Core.SPI.Interfaces;
using BankingService.Infra.Database.API.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Tests
{
    internal class BankDatabaseServiceTests
    {
        IBankDatabaseService bankDatabaseService_sut;
        Mock<Infra.Database.SPI.Interfaces.IFileSystemService> mockFileSystemService;

        [SetUp]
        public void Setup()
        {
            mockFileSystemService = new();
            bankDatabaseService_sut = new BankDatabaseService(mockFileSystemService.Object);
        }

        [Test]
        public void Should_get_operation_types()
        {
            // GIVEN
            mockFileSystemService
                .Setup(x => x.ReadAllLines(It.IsAny<string>()))
                .Returns(new List<string>
                {
                    "StringToScan;AssociatedType",
                    "PAYPAL;Paypal",
                    "VIR;Virement"
                });

            // WHEN
            var result = bankDatabaseService_sut.GetOperationTypes();

            // GIVEN
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result["VIR"], Is.EqualTo("Virement"));
            Assert.That(result["PAYPAL"], Is.EqualTo("Paypal"));
        }

        [Test]
        public void Should_get_operation_Categories_and_autoComment()
        {
            // GIVEN
            mockFileSystemService
                .Setup(x => x.ReadAllLines("Database/CategoriesAndAutoComments.csv"))
                .Returns(new List<string>
                {
                    "StringToScan;AssociatedType;AssociatedCommentAuto",
                    "AUCHAN;Nourriture;Courses (Auchan)",
                    "SNCF;Voyage/Deplacement;Train"
                });

            // WHEN
            var result = bankDatabaseService_sut.GetOperationCategoriesAndAutoComment();

            // GIVEN
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(result["AUCHAN"].Category, Is.EqualTo("Nourriture"));
                Assert.That(result["AUCHAN"].AutoComment, Is.EqualTo("Courses (Auchan)"));
                Assert.That(result["SNCF"].Category, Is.EqualTo("Voyage/Deplacement"));
                Assert.That(result["SNCF"].AutoComment, Is.EqualTo("Train"));
            });
        }
    }
}
