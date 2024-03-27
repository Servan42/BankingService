using BankingService.Core.API.Interfaces;
using BankingService.Core.API.Services;
using BankingService.Core.SPI.DTOs;
using BankingService.Core.SPI.Interfaces;
using Moq;

namespace BankingService.Tests
{
    public class ImportServiceTests
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
            importService_sut = new ImportService(fileSystemService.Object, bankDatabaseService.Object);
        }

        [TestCase("-20,45","", -20.45)]
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
                    Treasury = 766.87m
                }
            };
            bankDatabaseService.Verify(x => x.InsertOperationsIfNew(It.Is<List<OperationDto>>(o => CheckOperation(o, expected))), Times.Once());
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
                    Type = "Sans Contact"
                }
            };
            bankDatabaseService.Verify(x => x.InsertOperationsIfNew(It.Is<List<OperationDto>>(o => CheckOperation(o, expected))), Times.Once());
        }

        private bool CheckOperation(List<OperationDto> actual, List<OperationDto> expected)
        {
            Assert.That(actual.Count, Is.EqualTo(expected.Count), "Not the same amount of elements");
            for (int i = 0; i < actual.Count; i++)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(actual[i].Date, Is.EqualTo(expected[i].Date));
                    Assert.That(actual[i].Flow, Is.EqualTo(expected[i].Flow));
                    Assert.That(actual[i].Treasury, Is.EqualTo(expected[i].Treasury));
                    Assert.That(actual[i].Type, Is.EqualTo(expected[i].Type));
                    Assert.That(actual[i].Comment, Is.EqualTo(expected[i].Comment));
                    Assert.That(actual[i].AutoComment, Is.EqualTo(expected[i].AutoComment));
                    Assert.That(actual[i].Category, Is.EqualTo(expected[i].Category));
                    Assert.That(actual[i].Label, Is.EqualTo(expected[i].Label));
                });
            }

            return true;
        }
    }
}