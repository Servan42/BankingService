﻿using BankingService.Core.SPI.DTOs;
using BankingService.Core.SPI.Interfaces;
using BankingService.Infra.Database.Services;
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

        [Test]
        public void Should_insert_line_if_new()
        {
            // GIVEN
            var operations = new List<OperationDto>
            {
                new OperationDto 
                {
                    Date = new DateTime(2024,03,24),
                    Flow = -10.01m,
                    Label = "label1",
                    Treasury = 20m
                },
                new OperationDto
                {
                    Date = new DateTime(2024,03,24),
                    Flow = -10.01m,
                    Label = "label2",
                    Treasury = 20m
                },
            };

            mockFileSystemService
                .Setup(x => x.ReadAllLines("Database/Operations.csv"))
                .Returns(new List<string>
                {
                    "Date;Flow;Treasury;Label;Type;Category;AutoComment;Comment",
                    "2024-03-24;-10,01;20,00;label1;;;;"
                });

            // WHEN
            bankDatabaseService_sut.InsertOperationsIfNew(operations);

            // THEN
            var expected = new List<string>
            {
                "Date;Flow;Treasury;Label;Type;Category;AutoComment;Comment",
                "2024-03-24;-10,01;20,00;label1;;;;",
                "2024-03-24;-10,01;20,00;label2;;;;"
            };
            mockFileSystemService.Verify(x => x.WriteAllLinesOverride("Database/Operations.csv", It.Is<List<string>>(o => CheckOperations(o, expected))));
        }

        private bool CheckOperations(List<string> operations, List<string> expected)
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
