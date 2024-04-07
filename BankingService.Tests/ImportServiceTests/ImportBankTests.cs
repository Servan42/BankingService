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
    }
}