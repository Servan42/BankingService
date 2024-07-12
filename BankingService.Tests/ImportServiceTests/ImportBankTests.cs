using AutoMapper;
using BankingService.Core.API.Interfaces;
using BankingService.Core.API.MapperProfile;
using BankingService.Core.Exceptions;
using BankingService.Core.Services;
using BankingService.Core.SPI.DTOs;
using BankingService.Core.SPI.Interfaces;
using BankingService.Core.SPI.MapperProfile;
using Moq;

namespace BankingService.Tests.ImportServiceTests
{
    public class ImportBankTests
    {
        Mock<IFileSystemServiceForCore> fileSystemService;
        Mock<IBankDatabaseService> bankDatabaseService;
        IImportService importService_sut;

        [SetUp]
        public void Setup()
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CoreSpiProfile>();
                cfg.AddProfile<CoreApiProfile>();
            }));
            fileSystemService = new Mock<IFileSystemServiceForCore>();
            bankDatabaseService = new Mock<IBankDatabaseService>();
            bankDatabaseService.Setup(x => x.GetTransactionTypesKvp()).Returns([]);
            bankDatabaseService.Setup(x => x.GetTransactionCategoriesAndAutoCommentKvp()).Returns([]);
            importService_sut = new ImportService(fileSystemService.Object, bankDatabaseService.Object, mapper);
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
            var expected = new List<TransactionDto>
            {
                new TransactionDto
                {
                    Id = null,
                    Date = new DateTime(2023,11,21),
                    Flow = expectedFlow,
                    Label = "PAIEMENT PSC 2011 GRENOBLE AUCHAN GRENOBLE CARTE 6888",
                    Treasury = 766.87m,
                    Type = "TODO",
                    Category = "TODO",
                    AutoComment = ""
                }
            };
            bankDatabaseService.Verify(x => x.InsertTransactionsIfNew(It.Is<List<TransactionDto>>(o => o.IsEqualTo(expected))), Times.Once());
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
                .Setup(x => x.GetTransactionTypesKvp())
                .Returns(new Dictionary<string, string>
                {
                    { "PSC", "Sans Contact" }
                });

            // WHEN
            importService_sut.ImportBankFile("bankFilePath.csv");

            // THEN
            var expected = new List<TransactionDto>
            {
                new TransactionDto
                {
                    Id = null,
                    Date = new DateTime(2023,11,21),
                    Flow = -20.47m,
                    Label = "PAIEMENT PSC 2011 GRENOBLE AUCHAN GRENOBLE CARTE 6888",
                    Treasury = 766.87m,
                    Type = "Sans Contact",
                    Category = "TODO",
                    AutoComment = ""
                }
            };
            bankDatabaseService.Verify(x => x.InsertTransactionsIfNew(It.Is<List<TransactionDto>>(o => o.IsEqualTo(expected))), Times.Once());
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
                .Setup(x => x.GetTransactionCategoriesAndAutoCommentKvp())
                .Returns(new Dictionary<string, TransactionCategoryAndAutoCommentDto>
                {
                    { "AUCHAN", new TransactionCategoryAndAutoCommentDto { Category = "Nourriture", AutoComment = "Courses (Auchan)" } },
                });

            // WHEN
            importService_sut.ImportBankFile("bankFilePath.csv");

            // THEN
            var expected = new List<TransactionDto>
            {
                new TransactionDto
                {
                    Id = null,
                    Date = new DateTime(2023,11,21),
                    Flow = -20.47m,
                    Label = "PAIEMENT PSC 2011 GRENOBLE AUCHAN GRENOBLE CARTE 6888",
                    Treasury = 766.87m,
                    Type = "TODO",
                    AutoComment = "Courses (Auchan)",
                    Category = "Nourriture"
                }
            };
            bankDatabaseService.Verify(x => x.InsertTransactionsIfNew(It.Is<List<TransactionDto>>(o => o.IsEqualTo(expected))), Times.Once());
        }

        [Test]
        public void Should_throw_exception_when_not_a_bank_file()
        {
            // GIVEN
            fileSystemService
                .Setup(x => x.ReadAllLines("bankFilePath.csv"))
                .Returns(new List<string>
                {
                    "line1",
                    "line2"
                });

            // WHEN
            var ex = Assert.Throws<BusinessException>(() => importService_sut.ImportBankFile("bankFilePath.csv"));

            // THEN
            Assert.That(ex.Message, Is.EqualTo("Not recognized as a bank CSV file."));
        }

        [Test]
        public void Should_throw_exception_when_missing_a_feild()
        {
            // GIVEN
            // GIVEN
            fileSystemService
                .Setup(x => x.ReadAllLines("bankFilePath.csv"))
                .Returns(new List<string>
                {
                    "Date;Date de valeur;Débit;Crédit;Libellé;Solde",
                    $"21/11/2023;22/11/2023;-20,47;PAIEMENT PSC 2011 GRENOBLE AUCHAN GRENOBLE CARTE 6888;766,87"
                });

            // WHEN
            var ex = Assert.Throws<BusinessException>(() => importService_sut.ImportBankFile("bankFilePath.csv"));

            // THEN
            Assert.That(ex.Message, Is.EqualTo("The line \"21/11/2023;22/11/2023;-20,47;PAIEMENT PSC 2011 GRENOBLE AUCHAN GRENOBLE CARTE 6888;766,87\" contains 5 fields. Expected 6."));
        }

        [Test]
        public void Should_throw_exception_cannot_parse_transaction_date()
        {
            // GIVEN
            // GIVEN
            fileSystemService
                .Setup(x => x.ReadAllLines("bankFilePath.csv"))
                .Returns(new List<string>
                {
                    "Date;Date de valeur;Débit;Crédit;Libellé;Solde",
                    $"2111/2023;22/11/2023;-20,47;;PAIEMENT PSC 2011 GRENOBLE AUCHAN GRENOBLE CARTE 6888;766,87"
                });

            // WHEN
            var ex = Assert.Throws<BusinessException>(() => importService_sut.ImportBankFile("bankFilePath.csv"));

            // THEN
            Assert.That(ex.Message, Is.EqualTo("The line \"2111/2023;22/11/2023;-20,47;;PAIEMENT PSC 2011 GRENOBLE AUCHAN GRENOBLE CARTE 6888;766,87\" could not be parsed: String '2111/2023' was not recognized as a valid DateTime."));
        }

        [TestCase("-20,4a7", "")]
        [TestCase("", "20,4a7")]
        public void Should_throw_exception_cannot_parse_transaction_flow(string negative, string positive)
        {
            // GIVEN
            // GIVEN
            fileSystemService
                .Setup(x => x.ReadAllLines("bankFilePath.csv"))
                .Returns(new List<string>
                {
                    "Date;Date de valeur;Débit;Crédit;Libellé;Solde",
                    $"21/11/2023;22/11/2023;{negative};{positive};PAIEMENT PSC 2011 GRENOBLE AUCHAN GRENOBLE CARTE 6888;766,87"
                });

            // WHEN
            var ex = Assert.Throws<BusinessException>(() => importService_sut.ImportBankFile("bankFilePath.csv"));

            // THEN
            var faultyString = negative == string.Empty ? positive : negative;
            Assert.That(ex.Message, Is.EqualTo($"The line \"21/11/2023;22/11/2023;{negative};{positive};PAIEMENT PSC 2011 GRENOBLE AUCHAN GRENOBLE CARTE 6888;766,87\" could not be parsed: The input string '{faultyString}' was not in a correct format."));
        }

        [Test]
        public void Should_throw_exception_cannot_parse_transaction_treasury()
        {
            // GIVEN
            // GIVEN
            fileSystemService
                .Setup(x => x.ReadAllLines("bankFilePath.csv"))
                .Returns(new List<string>
                {
                    "Date;Date de valeur;Débit;Crédit;Libellé;Solde",
                    $"21/11/2023;22/11/2023;-20,47;;PAIEMENT PSC 2011 GRENOBLE AUCHAN GRENOBLE CARTE 6888;76a6,87"
                });

            // WHEN
            var ex = Assert.Throws<BusinessException>(() => importService_sut.ImportBankFile("bankFilePath.csv"));

            // THEN
            Assert.That(ex.Message, Is.EqualTo("The line \"21/11/2023;22/11/2023;-20,47;;PAIEMENT PSC 2011 GRENOBLE AUCHAN GRENOBLE CARTE 6888;76a6,87\" could not be parsed: The input string '76a6,87' was not in a correct format."));
        }
    }
}