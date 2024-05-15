using BankingService.Core.SPI.DTOs;
using BankingService.Core.SPI.Interfaces;
using BankingService.Infra.Database.Services;
using BankingService.Infra.Database.SPI.Interfaces;
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
        Mock<IBankDatabaseConfiguration> mockDatabaseConfiguration;
        private readonly string KEY = "key";
        private readonly string DB_PATH = "dbPath";

        [SetUp]
        public void Setup()
        {
            mockFileSystemService = new();
            mockDatabaseConfiguration = new();
            mockDatabaseConfiguration.Setup(x => x.DatabaseKey).Returns(KEY);
            mockDatabaseConfiguration.Setup(x => x.DatabasePath).Returns(DB_PATH);
            bankDatabaseService_sut = new BankDatabaseService(mockFileSystemService.Object, mockDatabaseConfiguration.Object);
        }

        [Test]
        public void Should_get_operation_types()
        {
            // GIVEN
            var tFile = Path.Combine(DB_PATH, "Database", "Types.csv");
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
            var caFile = Path.Combine(DB_PATH, "Database", "CategoriesAndAutoComments.csv");
            var cFile = Path.Combine(DB_PATH, "Database", "Categories.csv");
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
            var pcFile = Path.Combine(DB_PATH, "Database", "PaypalCategories.csv");
            var cFile = Path.Combine(DB_PATH, "Database", "Categories.csv");
            mockFileSystemService
                .Setup(x => x.ReadAllLines(pcFile))
                .Returns(new List<string>
                {
                    "StringToScan;AssociatedCategoryId",
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
            string opFile = Path.Combine(DB_PATH, "Database", "Operations.table");
            var cFile = Path.Combine(DB_PATH, "Database", "Categories.csv");

            var operations = new List<OperationDto>
            {
                new OperationDto 
                {
                    Date = new DateTime(2024,03,24),
                    Flow = -10.01m,
                    Label = "label1",
                    Treasury = 20m,
                    Category = "TODO"
                },
                new OperationDto
                {
                    Date = new DateTime(2024,03,24),
                    Flow = -10.01m,
                    Label = "label2",
                    Treasury = 20m,
                    Category = "Loisir"
                },
            };

            mockFileSystemService
                .Setup(x => x.ReadAllLinesDecrypt(opFile, KEY))
                .Returns(new List<string>
                {
                    "Id;Date;Flow;Treasury;Label;Type;CategoryId;AutoComment;Comment",
                    "10;2024-03-24;-10,01;20,00;label1;;1;;"
                });

            mockFileSystemService
                .Setup(x => x.ReadAllLines(cFile))
                .Returns(new List<string>
                {
                    "Id;Name",
                    "1;TODO",
                    "2;Loisir",
                });

            // WHEN
            bankDatabaseService_sut.InsertOperationsIfNew(operations);

            // THEN
            var expected = new List<string>
            {
                "Id;Date;Flow;Treasury;Label;Type;CategoryId;AutoComment;Comment",
                "10;2024-03-24;-10,01;20,00;label1;;1;;",
                "11;2024-03-24;-10,01;20,00;label2;;2;;"
            };
            mockFileSystemService.Verify(x => x.WriteAllLinesOverrideEncrypt(opFile, It.Is<List<string>>(o => TestHelpers.CheckStringList(o, expected)), KEY));
            mockFileSystemService.Verify(x => x.ReadAllLinesDecrypt(opFile, KEY), Times.Once());
            mockFileSystemService.Verify(x => x.ReadAllLines(cFile), Times.Once());
        }

        [Test]
        public void Should_get_unresolved_paypal_operations()
        {
            // GIVEN
            string opFile = Path.Combine(DB_PATH, "Database", "Operations.table");
            var cFile = Path.Combine(DB_PATH, "Database", "Categories.csv");
            mockFileSystemService
                .Setup(x => x.ReadAllLinesDecrypt(opFile, KEY))
                .Returns(new List<string>
                {
                    "Id;Date;Flow;Treasury;Label;Type;CategoryId;AutoComment;Comment",
                    "1;2024-03-23;-10,01;20,00;label0;;2;;",
                    "2;2024-03-24;-10,01;20,00;label1;TODO;1;;",
                    "3;2024-03-25;-10,01;20,00;label2;Paypal;1;;",
                    "4;2024-03-26;-10,01;20,00;label3;Paypal;1;Spotify;",
                });

            mockFileSystemService
                .Setup(x => x.ReadAllLines(cFile))
                .Returns(new List<string>
                {
                    "Id;Name",
                    "1;TODO",
                    "2;Loisir",
                });

            // WHEN
            var result = bankDatabaseService_sut.GetUnresolvedPaypalOperations();

            // THEN
            var expected = new List<OperationDto>
            {
                new OperationDto
                {
                    Id = 3,
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
            mockFileSystemService.Verify(x => x.ReadAllLinesDecrypt(opFile, KEY), Times.Once());
            mockFileSystemService.Verify(x => x.ReadAllLines(cFile), Times.Once());
        }

        [Test]
        public void Should_update_operations()
        {
            // GIVEN
            string opFile = Path.Combine(DB_PATH, "Database", "Operations.table");
            var cFile = Path.Combine(DB_PATH, "Database", "Categories.csv");

            var operations = new List<UpdatableOperationDto>
            {
                new UpdatableOperationDto
                {
                    Id = 2,
                    Type = "Paypal",
                    Category = "Loisir",
                    AutoComment = "Spotify",
                    Comment = ""
                }
            };

            mockFileSystemService
                .Setup(x => x.ReadAllLinesDecrypt(opFile, KEY))
                .Returns(new List<string>
                {
                    "Id;Date;Flow;Treasury;Label;Type;CategoryId;AutoComment;Comment",
                    "1;2024-03-24;-10,01;20,00;label1;TODO;1;;",
                    "2;2024-03-25;-10,01;30,00;label1;Paypal;1;;"
                });

            mockFileSystemService
                .Setup(x => x.ReadAllLines(cFile))
                .Returns(new List<string>
                {
                    "Id;Name",
                    "1;TODO",
                    "2;Loisir",
                });

            // WHEN
            bankDatabaseService_sut.UpdateOperations(operations);

            // THEN
            var expected = new List<string>
            {
                "Id;Date;Flow;Treasury;Label;Type;CategoryId;AutoComment;Comment",
                "1;2024-03-24;-10,01;20,00;label1;TODO;1;;",
                "2;2024-03-25;-10,01;30,00;label1;Paypal;2;Spotify;"
            };
            mockFileSystemService.Verify(x => x.WriteAllLinesOverrideEncrypt(opFile, It.Is<List<string>>(o => TestHelpers.CheckStringList(o, expected)), KEY));
            mockFileSystemService.Verify(x => x.ReadAllLinesDecrypt(opFile, KEY), Times.Once());
            mockFileSystemService.Verify(x => x.ReadAllLines(cFile), Times.Once());
        }

        [Test]
        public void Should_get_operation_that_need_manual_input()
        {
            // GIVEN
            string opFile = Path.Combine(DB_PATH, "Database", "Operations.table");
            var cFile = Path.Combine(DB_PATH, "Database", "Categories.csv");
            mockFileSystemService
                .Setup(x => x.ReadAllLinesDecrypt(opFile, KEY))
                .Returns(new List<string>
                {
                    "Id;Date;Flow;Treasury;Label;Type;CategoryId;AutoComment;Comment",
                    "1;2024-03-24;-10,01;20,00;label1;Sans Contact;1;;",
                    "2;2024-03-25;-10,01;30,00;label2;Paypal;2;Steam;"
                });

            mockFileSystemService
                .Setup(x => x.ReadAllLines(cFile))
                .Returns(new List<string>
                {
                    "Id;Name",
                    "1;TODO",
                    "2;Loisir",
                });

            // WHEN
            var result = bankDatabaseService_sut.GetOperationsThatNeedsManualInput();

            // THEN
            var expected = new List<OperationDto>
            {
                new OperationDto
                {
                    Id = 1,
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
            mockFileSystemService.Verify(x => x.ReadAllLinesDecrypt(opFile, KEY), Times.Once());
            mockFileSystemService.Verify(x => x.ReadAllLines(cFile), Times.Once());
        }

        [Test]
        public void Should_get_operation_all_operations()
        {
            // GIVEN
            string opFile = Path.Combine(DB_PATH, "Database", "Operations.table");
            var cFile = Path.Combine(DB_PATH, "Database", "Categories.csv");
            mockFileSystemService
                .Setup(x => x.ReadAllLinesDecrypt(opFile, KEY))
                .Returns(new List<string>
                {
                    "Id;Date;Flow;Treasury;Label;Type;CategoryId;AutoComment;Comment",
                    "1;2024-03-24;-10,01;20,00;label1;Sans Contact;1;;",
                    "2;2024-03-25;-10,01;30,00;label2;Paypal;2;Steam;"
                });

            mockFileSystemService
                .Setup(x => x.ReadAllLines(cFile))
                .Returns(new List<string>
                {
                    "Id;Name",
                    "1;TODO",
                    "2;Loisir",
                });

            // WHEN
            var result = bankDatabaseService_sut.GetAllOperations();

            // THEN
            var expected = new List<OperationDto>
            {
                new OperationDto
                {
                    Id = 1,
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
                    Id = 2,
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
            mockFileSystemService.Verify(x => x.ReadAllLinesDecrypt(opFile, KEY), Times.Once());
            mockFileSystemService.Verify(x => x.ReadAllLines(cFile), Times.Once());
        }

        [Test]
        public void Should_get_all_categories_names()
        {
            // GIVEN
            mockFileSystemService
                .Setup(x => x.ReadAllLines(Path.Combine(DB_PATH, "Database", "Categories.csv")))
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

        [Test]
        public void Should_get_operations_by_date()
        {
            // GIVEN
            string opFile = Path.Combine(DB_PATH, "Database", "Operations.table");
            var cFile = Path.Combine(DB_PATH, "Database", "Categories.csv");
            mockFileSystemService
                .Setup(x => x.ReadAllLinesDecrypt(opFile, KEY))
                .Returns(new List<string>
                {
                    "Id;Date;Flow;Treasury;Label;Type;CategoryId;AutoComment;Comment",
                    "1;2024-03-24;-10,01;21,00;label1;Sans Contact;1;;",
                    "2;2024-03-25;-10,02;22,00;label2;Paypal;2;Steam;Silksong",
                    "3;2024-03-26;-10,03;23,00;label3;Loyer;1;LoyerAutoComment;LoyerComment",
                    "4;2024-03-27;-10,04;24,00;label4;Paypal;2;Steam;"
                });

            mockFileSystemService
                .Setup(x => x.ReadAllLines(cFile))
                .Returns(new List<string>
                {
                    "Id;Name",
                    "1;Charges",
                    "2;Loisir",
                });

            // WHEN
            var result = bankDatabaseService_sut.GetOperationsBetweenDates(new DateTime(2024,03,25), new DateTime(2024,03,26));

            // THEN
            var expected = new List<OperationDto>
            {
                new OperationDto
                {
                    Id = 2,
                    Date = new DateTime(2024,03,25),
                    Flow = -10.02m,
                    Treasury = 22.00m,
                    Label = "label2",
                    Type = "Paypal",
                    Category = "Loisir",
                    AutoComment = "Steam",
                    Comment = "Silksong"
                },
                new OperationDto
                {
                    Id = 3,
                    Date = new DateTime(2024,03,26),
                    Flow = -10.03m,
                    Treasury = 23.00m,
                    Label = "label3",
                    Type = "Loyer",
                    Category = "Charges",
                    AutoComment = "LoyerAutoComment",
                    Comment = "LoyerComment"
                }
            };

            Assert.That(TestHelpers.CheckOperationDtos(result, expected));
            mockFileSystemService.Verify(x => x.ReadAllLinesDecrypt(opFile, KEY), Times.Once());
            mockFileSystemService.Verify(x => x.ReadAllLines(cFile), Times.Once());
        }
    }
}
