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
            var tFile = "Database/Types.csv";
            mockFileSystemService
                .Setup(x => x.ReadAllLines(tFile))
                .Returns(new List<string>
                {
                    "StringToScan;AssociatedType",
                    "PAYPAL;Paypal",
                    "VIR;Virement"
                });

            // WHEN
            var result = bankDatabaseService_sut.GetOperationTypesKvp();

            // GIVEN
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result["VIR"], Is.EqualTo("Virement"));
            Assert.That(result["PAYPAL"], Is.EqualTo("Paypal"));
            mockFileSystemService.Verify(x => x.ReadAllLines(tFile), Times.Once());
        }

        [Test]
        public void Should_get_operation_Categories_and_autoComment()
        {
            // GIVEN
            var caFile = "Database/CategoriesAndAutoComments.csv";
            var cFile = "Database/Categories.csv";
            mockFileSystemService
                .Setup(x => x.ReadAllLines(caFile))
                .Returns(new List<string>
                {
                    "StringToScan;AssociatedCategory;AssociatedCommentAuto",
                    "AUCHAN;2;Courses (Auchan)",
                    "SNCF;1;Train"
                });

            mockFileSystemService
                .Setup(x => x.ReadAllLines(cFile))
                .Returns(new List<string>
                {
                    "Id;Name",
                    "1;Voyage/Deplacement",
                    "2;Nourriture",
                });

            // WHEN
            var result = bankDatabaseService_sut.GetOperationCategoriesAndAutoCommentKvp();

            // GIVEN
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(result["AUCHAN"].Category, Is.EqualTo("Nourriture"));
                Assert.That(result["AUCHAN"].AutoComment, Is.EqualTo("Courses (Auchan)"));
                Assert.That(result["SNCF"].Category, Is.EqualTo("Voyage/Deplacement"));
                Assert.That(result["SNCF"].AutoComment, Is.EqualTo("Train"));
            });
            mockFileSystemService.Verify(x => x.ReadAllLines(caFile), Times.Once());
            mockFileSystemService.Verify(x => x.ReadAllLines(cFile), Times.Once());
        }

        [Test]
        public void Should_get_Paypal_Categories()
        {
            // GIVEN
            var pcFile = "Database/PaypalCategories.csv";
            var cFile = "Database/Categories.csv";
            mockFileSystemService
                .Setup(x => x.ReadAllLines(pcFile))
                .Returns(new List<string>
                {
                    "StringToScan;AssociatedCategory",
                    "Spotify;2",
                    "Zwift;1"
                });

            mockFileSystemService
                .Setup(x => x.ReadAllLines(cFile))
                .Returns(new List<string>
                {
                    "Id;Name",
                    "1;Sport",
                    "2;Loisirs",
                });

            // WHEN
            var result = bankDatabaseService_sut.GetPaypalCategoriesKvp();

            // GIVEN
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(result["Spotify"], Is.EqualTo("Loisirs"));
                Assert.That(result["Zwift"], Is.EqualTo("Sport"));
            });
            mockFileSystemService.Verify(x => x.ReadAllLines(pcFile), Times.Once());
            mockFileSystemService.Verify(x => x.ReadAllLines(cFile), Times.Once());
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
            mockFileSystemService.Verify(x => x.WriteAllLinesOverride("Database/Operations.csv", It.Is<List<string>>(o => TestHelpers.CheckStringList(o, expected))));
        }

        [Test]
        public void Should_get_unresolved_paypal_operations()
        {
            // GIVEN
            mockFileSystemService
                .Setup(x => x.ReadAllLines("Database/Operations.csv"))
                .Returns(new List<string>
                {
                    "Date;Flow;Treasury;Label;Type;Category;AutoComment;Comment",
                    "2024-03-23;-10,01;20,00;label0;;;;",
                    "2024-03-24;-10,01;20,00;label1;TODO;TODO;;",
                    "2024-03-25;-10,01;20,00;label2;Paypal;TODO;;",
                    "2024-03-26;-10,01;20,00;label3;Paypal;TODO;Spotify;",
                });

            // WHEN
            var result = bankDatabaseService_sut.GetUnresolvedPaypalOperations();

            // THEN
            var expected = new List<OperationDto>
            {
                new OperationDto
                {
                    Date = new DateTime(2024,03,25),
                    Flow = -10.01m,
                    Treasury = 20.00m,
                    Label = "label2",
                    Type = "Paypal",
                    Category = "TODO",
                    AutoComment = "",
                    Comment = ""
                }
            };
            Assert.That(TestHelpers.CheckOperationDtos(result, expected));
        }

        [Test]
        public void Should_update_operations()
        {
            // GIVEN
            var operations = new List<OperationDto>
            {
                new OperationDto
                {
                    Date = new DateTime(2024,03,25),
                    Flow = -10.01m,
                    Label = "label1",
                    Treasury = 30m,
                    Type = "Paypal",
                    Category = "Loisir",
                    AutoComment = "Spotify",
                    Comment = ""
                }
            };

            mockFileSystemService
                .Setup(x => x.ReadAllLines("Database/Operations.csv"))
                .Returns(new List<string>
                {
                    "Date;Flow;Treasury;Label;Type;Category;AutoComment;Comment",
                    "2024-03-24;-10,01;20,00;label1;TODO;TODO;;",
                    "2024-03-25;-10,01;30,00;label1;Paypal;TODO;;"
                });

            // WHEN
            bankDatabaseService_sut.UpdateOperations(operations);

            // THEN
            var expected = new List<string>
            {
                "Date;Flow;Treasury;Label;Type;Category;AutoComment;Comment",
                "2024-03-24;-10,01;20,00;label1;TODO;TODO;;",
                "2024-03-25;-10,01;30,00;label1;Paypal;Loisir;Spotify;"
            };
            mockFileSystemService.Verify(x => x.WriteAllLinesOverride("Database/Operations.csv", It.Is<List<string>>(o => TestHelpers.CheckStringList(o, expected))));
        }

        [Test]
        public void Should_get_operation_that_need_manual_input()
        {
            // GIVEN
            mockFileSystemService
                .Setup(x => x.ReadAllLines("Database/Operations.csv"))
                .Returns(new List<string>
                {
                    "Date;Flow;Treasury;Label;Type;Category;AutoComment;Comment",
                    "2024-03-24;-10,01;20,00;label1;Sans Contact;TODO;;",
                    "2024-03-25;-10,01;30,00;label2;Paypal;Loisir;Steam;"
                });

            // WHEN
            var result = bankDatabaseService_sut.GetOperationsThatNeedsManualInput();

            // THEN
            var expected = new List<OperationDto>
            {
                new OperationDto
                {
                    Date = new DateTime(2024,03,24),
                    Flow = -10.01m,
                    Treasury = 20.00m,
                    Label = "label1",
                    Type = "Sans Contact",
                    Category = "TODO",
                    AutoComment = "",
                    Comment = ""
                }
            };
            Assert.That(TestHelpers.CheckOperationDtos(result, expected));
        }

        [Test]
        public void Should_get_operation_all_operations()
        {
            // GIVEN
            mockFileSystemService
                .Setup(x => x.ReadAllLines("Database/Operations.csv"))
                .Returns(new List<string>
                {
                    "Date;Flow;Treasury;Label;Type;Category;AutoComment;Comment",
                    "2024-03-24;-10,01;20,00;label1;Sans Contact;TODO;;",
                    "2024-03-25;-10,01;30,00;label2;Paypal;Loisir;Steam;"
                });

            // WHEN
            var result = bankDatabaseService_sut.GetAllOperations();

            // THEN
            var expected = new List<OperationDto>
            {
                new OperationDto
                {
                    Date = new DateTime(2024,03,24),
                    Flow = -10.01m,
                    Treasury = 20.00m,
                    Label = "label1",
                    Type = "Sans Contact",
                    Category = "TODO",
                    AutoComment = "",
                    Comment = ""
                },
                new OperationDto
                {
                    Date = new DateTime(2024,03,25),
                    Flow = -10.01m,
                    Treasury = 30.00m,
                    Label = "label2",
                    Type = "Paypal",
                    Category = "Loisir",
                    AutoComment = "Steam",
                    Comment = ""
                }
            };
            Assert.That(TestHelpers.CheckOperationDtos(result, expected));
        }

        [Test]
        public void Should_get_all_categories_names()
        {
            // GIVEN
            mockFileSystemService
                .Setup(x => x.ReadAllLines("Database/Categories.csv"))
                .Returns(new List<string>
                {
                    "Id;Name",
                    "1;Income",
                    "2;Autres",
                });

            // WHEN
            var result = this.bankDatabaseService_sut.GetAllCategoriesNames();

            // THEN
            CollectionAssert.AreEqual(new List<string> { "Income", "Autres" }, result);
        }
    }
}
