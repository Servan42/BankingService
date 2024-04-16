using BankingService.Core.API.Interfaces;
using BankingService.Core.Services;
using BankingService.Core.SPI.DTOs;
using BankingService.Core.SPI.Interfaces;
using Moq;

namespace BankingService.Tests.ImportServiceTests
{
    internal class ImportPaypalTests
    {
        Mock<IFileSystemService> fileSystemService;
        Mock<IBankDatabaseService> bankDatabaseService;
        IImportService importService_sut;

        [SetUp]
        public void Setup()
        {
            fileSystemService = new Mock<IFileSystemService>();
            bankDatabaseService = new Mock<IBankDatabaseService>();
            bankDatabaseService.Setup(x => x.GetUnresolvedPaypalOperations()).Returns([]);
            importService_sut = new ImportService(fileSystemService.Object, bankDatabaseService.Object);
        }

        [Test]
        public void Should_archive_paypal_import()
        {
            // GIVEN
            fileSystemService
                .Setup(x => x.ReadAllLines("folder/paypal.csv"))
                .Returns(new List<string>
                {
                    "\"Date\",\"Heure\",\"Fuseau horaire\",\"Description\",\"Devise\",\"Brut \",\"Frais \",\"Net\",\"Solde\",\"Numéro de transaction\",\"Adresse email de l'expéditeur\",\"Nom\",\"Nom de la banque\",\"Compte bancaire\",\"Montant des frais de livraison et de traitement\",\"TVA\",\"Numéro de facture\",\"Numéro de la transaction de référence\"",
                    "\"07/01/2024\",\"08:29:44\",\"Europe/Berlin\",\"Paiement préapprouvé d'un utilisateur de facture de paiement\",\"EUR\",\"-10,99\",\"0,00\",\"-10,99\",\"-10,99\",\"5678\",\"paypal-se@spotify.com\",\"Spotify AB\",\"\",\"\",\"0,00\",\"0,00\",\"P122\",\"B-1245\"",
                    "\"07/01/2024\",\"08:29:44\",\"Europe/Berlin\",\"Virement bancaire sur le compte PayPal\",\"EUR\",\"10,99\",\"0,00\",\"10,99\",\"0,00\",\"1234\",\"\",\"\",\"Caisse Federale de Banque\",\"7303\",\"0,00\",\"0,00\",\"P122\",\"5678\"\r\n"
                });

            // WHEN
            importService_sut.ImportPaypalFile("folder/paypal.csv");

            // THEN
            fileSystemService.Verify(x => x.ArchiveFile("folder/paypal.csv", "Archive/Paypal_Import"), Times.Once());
        }

        [Test]
        public void Should_integrate_paypal_data_to_output_for_exact_date()
        {
            // GIVEN
            fileSystemService
                .Setup(x => x.ReadAllLines("folder/paypal.csv"))
                .Returns(new List<string>
                {
                    "\"Date\",\"Heure\",\"Fuseau horaire\",\"Description\",\"Devise\",\"Brut \",\"Frais \",\"Net\",\"Solde\",\"Numéro de transaction\",\"Adresse email de l'expéditeur\",\"Nom\",\"Nom de la banque\",\"Compte bancaire\",\"Montant des frais de livraison et de traitement\",\"TVA\",\"Numéro de facture\",\"Numéro de la transaction de référence\"",
                    "\"07/01/2024\",\"08:29:44\",\"Europe/Berlin\",\"Paiement préapprouvé d'un utilisateur de facture de paiement\",\"EUR\",\"-10,99\",\"0,00\",\"-10,99\",\"-10,99\",\"5678\",\"paypal-se@spotify.com\",\"Spotify AB\",\"\",\"\",\"0,00\",\"0,00\",\"P122\",\"B-1245\"",
                    "\"07/01/2024\",\"08:29:44\",\"Europe/Berlin\",\"Virement bancaire sur le compte PayPal\",\"EUR\",\"10,99\",\"0,00\",\"10,99\",\"0,00\",\"1234\",\"\",\"\",\"Caisse Federale de Banque\",\"7303\",\"0,00\",\"0,00\",\"P122\",\"5678\"\r\n"
                });
            bankDatabaseService
                .Setup(x => x.GetUnresolvedPaypalOperations())
                .Returns(new List<OperationDto>
                {
                    new OperationDto { Date = new DateTime(2024,01,06), Flow = -10.99m }, // no match
                    new OperationDto { Date = new DateTime(2024,01,07), Flow = -9.99m }, // no match
                    new OperationDto { Date = new DateTime(2024,01,07), Flow = -10.99m, Treasury = 0, Label = "a", Type = "Paypal", AutoComment = "", Category = "TODO", Comment = "e" }
                });
            bankDatabaseService
                .Setup(x => x.GetPaypalCategoriesKvp())
                .Returns(new Dictionary<string, string>
                {
                    { "Spotify", "Loisirs" }
                });

            // WHEN
            importService_sut.ImportPaypalFile("folder/paypal.csv");

            // THEN
            var expected = new List<OperationDto>
            {
                new OperationDto { Date = new DateTime(2024,01,07), Flow = -10.99m, Treasury = 0, Label = "a", Type = "Paypal", AutoComment = "Spotify AB", Category = "Loisirs", Comment = "e" }
            };
            bankDatabaseService.Verify(x => x.UpdateOperations(It.Is<List<OperationDto>>(actual => TestHelpers.CheckOperationDtos(actual, expected))), Times.Once());
        }

        [Test]
        public void Should_correctly_match_paypal_data_when_two_lines_have_the_same_date_and_flow()
        {
            // GIVEN
            fileSystemService
                .Setup(x => x.ReadAllLines("folder/paypal.csv"))
                .Returns(new List<string>
                {
                    "\"Date\",\"Heure\",\"Fuseau horaire\",\"Description\",\"Devise\",\"Brut \",\"Frais \",\"Net\",\"Solde\",\"Numéro de transaction\",\"Adresse email de l'expéditeur\",\"Nom\",\"Nom de la banque\",\"Compte bancaire\",\"Montant des frais de livraison et de traitement\",\"TVA\",\"Numéro de facture\",\"Numéro de la transaction de référence\"",
                    "\"07/01/2024\",\"08:29:44\",\"Europe/Berlin\",\"Paiement préapprouvé d'un utilisateur de facture de paiement\",\"EUR\",\"-10,99\",\"0,00\",\"-10,99\",\"-10,99\",\"5678\",\"paypal-se@spotify.com\",\"Spotify AB\",\"\",\"\",\"0,00\",\"0,00\",\"P122\",\"B-1245\"",
                    "\"07/01/2024\",\"08:29:44\",\"Europe/Berlin\",\"Virement bancaire sur le compte PayPal\",\"EUR\",\"10,99\",\"0,00\",\"10,99\",\"0,00\",\"1234\",\"\",\"\",\"Caisse Federale de Banque\",\"7303\",\"0,00\",\"0,00\",\"P122\",\"5678\"",
                    "\"07/01/2024\",\"08:29:44\",\"Europe/Berlin\",\"Paiement préapprouvé d'un utilisateur de facture de paiement\",\"EUR\",\"-10,99\",\"0,00\",\"-10,99\",\"-10,99\",\"5678\",\"paypal-se@spotify.com\",\"Spotify AB 2\",\"\",\"\",\"0,00\",\"0,00\",\"P122\",\"B-1245\"",
                    "\"07/01/2024\",\"08:29:44\",\"Europe/Berlin\",\"Virement bancaire sur le compte PayPal\",\"EUR\",\"10,99\",\"0,00\",\"10,99\",\"0,00\",\"1234\",\"\",\"\",\"Caisse Federale de Banque\",\"7303\",\"0,00\",\"0,00\",\"P122\",\"5678\""
                });
            bankDatabaseService
                .Setup(x => x.GetUnresolvedPaypalOperations())
                .Returns(new List<OperationDto>
                {
                    new OperationDto { Date = new DateTime(2024,01,07), Flow = -10.99m, Treasury = 0, Label = "a", Type = "Paypal", AutoComment = "", Category = "TODO", Comment = "e" },
                    new OperationDto { Date = new DateTime(2024,01,07), Flow = -10.99m, Treasury = 100, Label = "a", Type = "Paypal", AutoComment = "", Category = "TODO", Comment = "e" }
                });
            bankDatabaseService
                .Setup(x => x.GetPaypalCategoriesKvp())
                .Returns(new Dictionary<string, string>
                {
                    { "Spotify", "Loisirs" }
                });

            // WHEN
            importService_sut.ImportPaypalFile("folder/paypal.csv");

            // THEN
            var expected = new List<OperationDto>
            {
                new OperationDto { Date = new DateTime(2024,01,07), Flow = -10.99m, Treasury = 0, Label = "a", Type = "Paypal", AutoComment = "Spotify AB", Category = "Loisirs", Comment = "e" },
                new OperationDto { Date = new DateTime(2024,01,07), Flow = -10.99m, Treasury = 100, Label = "a", Type = "Paypal", AutoComment = "Spotify AB 2", Category = "Loisirs", Comment = "e" }
            };
            bankDatabaseService.Verify(x => x.UpdateOperations(It.Is<List<OperationDto>>(actual => TestHelpers.CheckOperationDtos(actual, expected))), Times.Once());
        }

        [Test]
        public void Should_match_unmatched_paypal_data_with_incremental_date_offset()
        {
            // GIVEN
            fileSystemService
                .Setup(x => x.ReadAllLines("folder/paypal.csv"))
                .Returns(new List<string>
                {
                    "\"Date\",\"Heure\",\"Fuseau horaire\",\"Description\",\"Devise\",\"Brut \",\"Frais \",\"Net\",\"Solde\",\"Numéro de transaction\",\"Adresse email de l'expéditeur\",\"Nom\",\"Nom de la banque\",\"Compte bancaire\",\"Montant des frais de livraison et de traitement\",\"TVA\",\"Numéro de facture\",\"Numéro de la transaction de référence\"",
                    "\"07/01/2024\",\"08:29:44\",\"Europe/Berlin\",\"Paiement préapprouvé d'un utilisateur de facture de paiement\",\"EUR\",\"-10,99\",\"0,00\",\"-10,99\",\"-10,99\",\"5678\",\"paypal-se@spotify.com\",\"Spotify AB\",\"\",\"\",\"0,00\",\"0,00\",\"P122\",\"B-1245\"",
                    "\"07/01/2024\",\"08:29:44\",\"Europe/Berlin\",\"Virement bancaire sur le compte PayPal\",\"EUR\",\"10,99\",\"0,00\",\"10,99\",\"0,00\",\"1234\",\"\",\"\",\"Caisse Federale de Banque\",\"7303\",\"0,00\",\"0,00\",\"P122\",\"5678\"",
                    "\"25/01/2024\",\"08:29:44\",\"Europe/Berlin\",\"Paiement préapprouvé d'un utilisateur de facture de paiement\",\"EUR\",\"-22,99\",\"0,00\",\"-22,99\",\"-22,99\",\"5678\",\"steam.com\",\"Steam\",\"\",\"\",\"0,00\",\"0,00\",\"P122\",\"B-1245\"",
                    "\"25/01/2024\",\"08:29:44\",\"Europe/Berlin\",\"Virement bancaire sur le compte PayPal\",\"EUR\",\"22,99\",\"0,00\",\"22,99\",\"0,00\",\"1234\",\"\",\"\",\"Caisse Federale de Banque\",\"7303\",\"0,00\",\"0,00\",\"P122\",\"5678\""
                });
            bankDatabaseService
                .Setup(x => x.GetUnresolvedPaypalOperations())
                .Returns(new List<OperationDto>
                {
                    new OperationDto { Date = new DateTime(2024,01,10), Flow = -10.99m, Treasury = 0, Label = "a", Type = "Paypal", AutoComment = "", Category = "TODO", Comment = "e" },
                    new OperationDto { Date = new DateTime(2024,01,25), Flow = -22.99m, Treasury = 10, Label = "aa", Type = "Paypal", AutoComment = "", Category = "TODO", Comment = "e" }
                });
            bankDatabaseService
                .Setup(x => x.GetPaypalCategoriesKvp())
                .Returns(new Dictionary<string, string>
                {
                    { "Spotify", "Loisirs" },
                    { "Steam", "Loisirs2" }
                });

            // WHEN
            importService_sut.ImportPaypalFile("folder/paypal.csv");

            // THEN
            var expected = new List<OperationDto>
            {
                new OperationDto { Date = new DateTime(2024,01,25), Flow = -22.99m, Treasury = 10, Label = "aa", Type = "Paypal", AutoComment = "Steam", Category = "Loisirs2", Comment = "e" },
                new OperationDto { Date = new DateTime(2024,01,10), Flow = -10.99m, Treasury = 0, Label = "a", Type = "Paypal", AutoComment = "Spotify AB", Category = "Loisirs", Comment = "e" }
            };
            bankDatabaseService.Verify(x => x.UpdateOperations(It.Is<List<OperationDto>>(actual => TestHelpers.CheckOperationDtos(actual, expected))), Times.Once());
        }
    }
}
