using BankingService.Core.API.Interfaces;
using BankingService.Core.Services;
using BankingService.Core.SPI.DTOs;
using BankingService.Core.SPI.Interfaces;
using Moq;
using System.Diagnostics;

namespace BankingService.Tests.ImportServiceTests
{
    public class ImportBankTests
    {
        Mock<IFileSystemService> fileSystemService;
        Mock<IBankDatabaseService> bankDatabaseService;
        IImportService importService_sut;

        [SetUp]
        public void Setup()
        {
            fileSystemService = new Mock<IFileSystemService>();
            bankDatabaseService = new Mock<IBankDatabaseService>();
            bankDatabaseService.Setup(x => x.GetOperationTypes()).Returns([]);
            bankDatabaseService.Setup(x => x.GetOperationCategoriesAndAutoComment()).Returns([]);
            importService_sut = new ImportService(fileSystemService.Object, bankDatabaseService.Object);
        }

        [TestCase("-20,45", "", -20.45)]
        [TestCase("", "10,11", 10.11)]
        public void Should_import_banking_file_line(string debit, string credit, decimal expectedFlow)
        {
            // GIVEN
            fileSystemService
                .Setup(x => x.ReadAllLines("bankFilePath.csv"))
                .Returns(new List<string>
                {
                    "Date;Date de valeur;Débit;Crédit;Libellé;Solde",
                    $"21/11/2023;22/11/2023;{debit};{credit};PAIEMENT PSC 2011 GRENOBLE AUCHAN GRENOBLE CARTE 6888;766,87"
                });

            // WHEN
            importService_sut.ImportBankFile("bankFilePath.csv");

            // THEN
            var expected = new List<OperationDto>
            {
                new OperationDto
                {
                    Date = new DateTime(2023,11,21),
                    Flow = expectedFlow,
                    Label = "PAIEMENT PSC 2011 GRENOBLE AUCHAN GRENOBLE CARTE 6888",
                    Treasury = 766.87m,
                    Type = "TODO",
                    Category = "TODO",
                    AutoComment = ""
                }
            };
            bankDatabaseService.Verify(x => x.InsertOperationsIfNew(It.Is<List<OperationDto>>(o => TestHelpers.CheckOperationDtos(o, expected))), Times.Once());
        }

        [Test]
        public void Should_archive_bank_import()
        {
            // GIVEN
            fileSystemService
                .Setup(x => x.ReadAllLines("folder/bankFilePath.csv"))
                .Returns(new List<string>
                {
                    "Date;Date de valeur;Débit;Crédit;Libellé;Solde",
                    $"21/11/2023;22/11/2023;-20,00;;PAIEMENT PSC 2011 GRENOBLE AUCHAN GRENOBLE CARTE 6888;766,87"
                });

            // WHEN
            importService_sut.ImportBankFile("folder/bankFilePath.csv");

            // THEN
            fileSystemService.Verify(x => x.ArchiveFile("folder/bankFilePath.csv", "Archive/Bank_Import"), Times.Once());
        }

        [Test]
        public void Should_import_banking_file_line_with_resolved_type()
        {
            // GIVEN
            fileSystemService
                .Setup(x => x.ReadAllLines("bankFilePath.csv"))
                .Returns(new List<string>
                {
                    "Date;Date de valeur;Débit;Crédit;Libellé;Solde",
                    $"21/11/2023;22/11/2023;-20,47;;PAIEMENT PSC 2011 GRENOBLE AUCHAN GRENOBLE CARTE 6888;766,87"
                });
            bankDatabaseService
                .Setup(x => x.GetOperationTypes())
                .Returns(new Dictionary<string, string>
                {
                    { "PSC", "Sans Contact" }
                });

            // WHEN
            importService_sut.ImportBankFile("bankFilePath.csv");

            // THEN
            var expected = new List<OperationDto>
            {
                new OperationDto
                {
                    Date = new DateTime(2023,11,21),
                    Flow = -20.47m,
                    Label = "PAIEMENT PSC 2011 GRENOBLE AUCHAN GRENOBLE CARTE 6888",
                    Treasury = 766.87m,
                    Type = "Sans Contact",
                    Category = "TODO",
                    AutoComment = ""
                }
            };
            bankDatabaseService.Verify(x => x.InsertOperationsIfNew(It.Is<List<OperationDto>>(o => TestHelpers.CheckOperationDtos(o, expected))), Times.Once());
        }

        [Test]
        public void Should_import_banking_file_line_with_resolved_category_and_autocomments()
        {
            // GIVEN
            fileSystemService
                .Setup(x => x.ReadAllLines("bankFilePath.csv"))
                .Returns(new List<string>
                {
                    "Date;Date de valeur;Débit;Crédit;Libellé;Solde",
                    $"21/11/2023;22/11/2023;-20,47;;PAIEMENT PSC 2011 GRENOBLE AUCHAN GRENOBLE CARTE 6888;766,87"
                });
            bankDatabaseService
                .Setup(x => x.GetOperationCategoriesAndAutoComment())
                .Returns(new Dictionary<string, OperationCategoryAndAutoCommentDto>
                {
                    { "AUCHAN", new OperationCategoryAndAutoCommentDto { Category = "Nourriture", AutoComment = "Courses (Auchan)" } },
                });

            // WHEN
            importService_sut.ImportBankFile("bankFilePath.csv");

            // THEN
            var expected = new List<OperationDto>
            {
                new OperationDto
                {
                    Date = new DateTime(2023,11,21),
                    Flow = -20.47m,
                    Label = "PAIEMENT PSC 2011 GRENOBLE AUCHAN GRENOBLE CARTE 6888",
                    Treasury = 766.87m,
                    Type = "TODO",
                    AutoComment = "Courses (Auchan)",
                    Category = "Nourriture"
                }
            };
            bankDatabaseService.Verify(x => x.InsertOperationsIfNew(It.Is<List<OperationDto>>(o => TestHelpers.CheckOperationDtos(o, expected))), Times.Once());
        }

        [Test]
        public void Should_recompute_every_line_additional_data()
        {
            // GIVEN
            bankDatabaseService.Setup(x => x.GetAllOperations()).Returns(new List<OperationDto>
            {
                new OperationDto { Date = new DateTime(2024, 10, 20), Flow = 1m, Treasury = 2m, Label = "PSC AUCHAN", Type = "Sans Contact", Category = "Nourriture", AutoComment = "Courses (Auchan)", Comment = "aaa" },
                new OperationDto { Date = new DateTime(2024, 10, 21), Flow = 1m, Treasury = 3m, Label = "PSC AUCHAN", Type = "TODO", Category = "TODO", AutoComment = "", Comment = "bbb" },
                new OperationDto { Date = new DateTime(2024, 10, 22), Flow = 1m, Treasury = 4m, Label = "PSC AUCHAN", Type = "a", Category = "b", AutoComment = "c", Comment = "ccc" },
                new OperationDto { Date = new DateTime(2024, 10, 23), Flow = 1m, Treasury = 5m, Label = "AAA", Type = "TODO", Category = "TODO", AutoComment = "", Comment = "ddd" },
                new OperationDto { Date = new DateTime(2024, 10, 24), Flow = 1m, Treasury = 6m, Label = "PSC BBB", Type = "Sans Contact", Category = "Special", AutoComment = "", Comment = "eee" },
                new OperationDto { Date = new DateTime(2024, 10, 25), Flow = 1m, Treasury = 7m, Label = "PSC CCC", Type = "Virement", Category = "Special 2", AutoComment = "", Comment = "fff" },
                new OperationDto { Date = new DateTime(2024, 10, 26), Flow = 1m, Treasury = 8m, Label = "PAYPAL", Type = "Paypal", Category = "TODO", AutoComment = "Spotify AB", Comment = "ggg" },
                new OperationDto { Date = new DateTime(2024, 10, 27), Flow = 1m, Treasury = 9m, Label = "PAYPAL", Type = "Paypal", Category = "TODO", AutoComment = "nomatch", Comment = "hhh" },
                new OperationDto { Date = new DateTime(2024, 10, 28), Flow = 1m, Treasury = 10m, Label = "PAYPAL", Type = "Paypal", Category = "TODO", AutoComment = "", Comment = "iii" },
                new OperationDto { Date = new DateTime(2024, 10, 29), Flow = 1m, Treasury = 11m, Label = "PSC NEW", Type = "Sans Contact", Category = "Manually categorized", AutoComment = "", Comment = "jjj" },
                new OperationDto { Date = new DateTime(2024, 10, 30), Flow = 1m, Treasury = 12m, Label = "PAYPAL", Type = "Paypal", Category = "Manually categorized paypal", AutoComment = "newMatch", Comment = "kkk" },
            });
            bankDatabaseService.Setup(x => x.GetOperationTypes()).Returns(new Dictionary<string, string>
            {
                { "PSC", "Sans Contact" },
                { "PAYPAL", "Paypal" }
            });
            bankDatabaseService.Setup(x => x.GetOperationCategoriesAndAutoComment()).Returns(new Dictionary<string, OperationCategoryAndAutoCommentDto>
            {
                { "AUCHAN", new OperationCategoryAndAutoCommentDto { Category = "Nourriture", AutoComment = "Courses (Auchan)" } },
                { "NEW", new OperationCategoryAndAutoCommentDto { Category = "NewCat", AutoComment = "NewComment" } }
            });
            bankDatabaseService.Setup(x => x.GetPaypalCategories()).Returns(new Dictionary<string, string>
            {
                { "Spotify", "Loisirs" },
                { "newMatch", "NewPaypalCat" }
            });

            // WHEN
            importService_sut.RecomputeEveryOperationAdditionalData();

            // THEN
            var expected = new List<OperationDto>
            {
                new OperationDto { Date = new DateTime(2024, 10, 20), Flow = 1m, Treasury = 2m, Label = "PSC AUCHAN", Type = "Sans Contact", Category = "Nourriture", AutoComment = "Courses (Auchan)", Comment = "aaa" },
                new OperationDto { Date = new DateTime(2024, 10, 21), Flow = 1m, Treasury = 3m, Label = "PSC AUCHAN", Type = "Sans Contact", Category = "Nourriture", AutoComment = "Courses (Auchan)", Comment = "bbb" },
                new OperationDto { Date = new DateTime(2024, 10, 22), Flow = 1m, Treasury = 4m, Label = "PSC AUCHAN", Type = "Sans Contact", Category = "Nourriture", AutoComment = "Courses (Auchan)", Comment = "ccc" },
                new OperationDto { Date = new DateTime(2024, 10, 23), Flow = 1m, Treasury = 5m, Label = "AAA", Type = "TODO", Category = "TODO", AutoComment = "", Comment = "ddd" },
                new OperationDto { Date = new DateTime(2024, 10, 24), Flow = 1m, Treasury = 6m, Label = "PSC BBB", Type = "Sans Contact", Category = "Special", AutoComment = "", Comment = "eee" },
                new OperationDto { Date = new DateTime(2024, 10, 25), Flow = 1m, Treasury = 7m, Label = "PSC CCC", Type = "Sans Contact", Category = "Special 2", AutoComment = "", Comment = "fff" },
                new OperationDto { Date = new DateTime(2024, 10, 26), Flow = 1m, Treasury = 8m, Label = "PAYPAL", Type = "Paypal", Category = "Loisirs", AutoComment = "Spotify AB", Comment = "ggg" },
                new OperationDto { Date = new DateTime(2024, 10, 27), Flow = 1m, Treasury = 9m, Label = "PAYPAL", Type = "Paypal", Category = "TODO", AutoComment = "nomatch", Comment = "hhh" },
                new OperationDto { Date = new DateTime(2024, 10, 28), Flow = 1m, Treasury = 10m, Label = "PAYPAL", Type = "Paypal", Category = "TODO", AutoComment = "", Comment = "iii" },
                new OperationDto { Date = new DateTime(2024, 10, 29), Flow = 1m, Treasury = 11m, Label = "PSC NEW", Type = "Sans Contact", Category = "NewCat", AutoComment = "NewComment", Comment = "jjj" },
                new OperationDto { Date = new DateTime(2024, 10, 30), Flow = 1m, Treasury = 12m, Label = "PAYPAL", Type = "Paypal", Category = "NewPaypalCat", AutoComment = "newMatch", Comment = "kkk" },
            };
            bankDatabaseService.Verify(x => x.UpdateOperations(It.Is<List<OperationDto>>(actual => TestHelpers.CheckOperationDtos(actual, expected))), Times.Once());
        }
    }
}