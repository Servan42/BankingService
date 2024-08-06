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
    internal class ImportPaypalTests
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
            bankDatabaseService.Setup(x => x.GetUnresolvedPaypalTransactions()).Returns([]);
            importService_sut = new ImportService(fileSystemService.Object, bankDatabaseService.Object, mapper);
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
                    "\"07/01/2024\",\"08:29:44\",\"Europe/Berlin\",\"Virement bancaire sur le compte PayPal\",\"EUR\",\"10,99\",\"0,00\",\"10,99\",\"0,00\",\"1234\",\"\",\"\",\"Caisse Federale de Banque\",\"7303\",\"0,00\",\"0,00\",\"P122\",\"5678\""
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
                    "\"07/01/2024\",\"08:29:44\",\"Europe/Berlin\",\"Virement bancaire sur le compte PayPal\",\"EUR\",\"10,99\",\"0,00\",\"10,99\",\"0,00\",\"1234\",\"\",\"\",\"Caisse Federale de Banque\",\"7303\",\"0,00\",\"0,00\",\"P122\",\"5678\""
                });
            bankDatabaseService
                .Setup(x => x.GetUnresolvedPaypalTransactions())
                .Returns(new List<TransactionDto>
                {
                    new TransactionDto { Id = 1, Date = new DateTime(2024,01,06), Flow = -10.99m }, // no match
                    new TransactionDto { Id = 2, Date = new DateTime(2024,01,07), Flow = -9.99m }, // no match
                    new TransactionDto { Id = 3, Date = new DateTime(2024,01,07), Flow = -10.99m, Treasury = 0, Label = "a", Type = "Paypal", AutoComment = "", Category = "TODO", Comment = "e" }
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
            var expected = new List<UpdatableTransactionDto>
            {
                new UpdatableTransactionDto { Id = 3, Type = "Paypal", AutoComment = "Spotify AB", Category = "Loisirs", Comment = "e" }
            };
            bankDatabaseService.Verify(x => x.UpdateTransactions(It.Is<List<UpdatableTransactionDto>>(actual => actual.IsEqualTo(expected))), Times.Once());
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
                .Setup(x => x.GetUnresolvedPaypalTransactions())
                .Returns(new List<TransactionDto>
                {
                    new TransactionDto { Id = 1, Date = new DateTime(2024,01,07), Flow = -10.99m, Treasury = 0, Label = "a", Type = "Paypal", AutoComment = "", Category = "TODO", Comment = "e" },
                    new TransactionDto { Id = 2, Date = new DateTime(2024,01,07), Flow = -10.99m, Treasury = 100, Label = "a", Type = "Paypal", AutoComment = "", Category = "TODO", Comment = "e" }
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
            var expected = new List<UpdatableTransactionDto>
            {
                new UpdatableTransactionDto { Id = 1, Type = "Paypal", AutoComment = "Spotify AB", Category = "Loisirs", Comment = "e" },
                new UpdatableTransactionDto { Id = 2, Type = "Paypal", AutoComment = "Spotify AB 2", Category = "Loisirs", Comment = "e" }
            };
            bankDatabaseService.Verify(x => x.UpdateTransactions(It.Is<List<UpdatableTransactionDto>>(actual => actual.IsEqualTo(expected))), Times.Once());
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
                .Setup(x => x.GetUnresolvedPaypalTransactions())
                .Returns(new List<TransactionDto>
                {
                    new TransactionDto { Id = 1, Date = new DateTime(2024,01,10), Flow = -10.99m, Treasury = 0, Label = "a", Type = "Paypal", AutoComment = "", Category = "TODO", Comment = "e" },
                    new TransactionDto { Id = 2, Date = new DateTime(2024,01,25), Flow = -22.99m, Treasury = 10, Label = "aa", Type = "Paypal", AutoComment = "", Category = "TODO", Comment = "e" }
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
            var expected = new List<UpdatableTransactionDto>
            {
                new UpdatableTransactionDto { Id = 2, Type = "Paypal", AutoComment = "Steam", Category = "Loisirs2", Comment = "e" },
                new UpdatableTransactionDto { Id = 1, Type = "Paypal", AutoComment = "Spotify AB", Category = "Loisirs", Comment = "e" }
            };
            bankDatabaseService.Verify(x => x.UpdateTransactions(It.Is<List<UpdatableTransactionDto>>(actual => actual.IsEqualTo(expected))), Times.Once());
        }

        [Test]
        public void Should_throw_exception_when_not_a_paypal_file()
        {
            // GIVEN
            fileSystemService
                .Setup(x => x.ReadAllLines("folder/paypal.csv"))
                .Returns(new List<string>
                {
                    "line1",
                    "line2"
                });

            // WHEN
            var ex = Assert.Throws<BusinessException>(() => importService_sut.ImportPaypalFile("folder/paypal.csv"));

            // THEN
            Assert.That(ex.Message, Is.EqualTo("Not recognized as a paypal CSV file."));
        }

        [Test]
        public void Should_throw_exception_when_missing_a_feild()
        {
            // GIVEN
            fileSystemService
                .Setup(x => x.ReadAllLines("folder/paypal.csv"))
                .Returns(new List<string>
                {
                    "\"Date\",\"Heure\",\"Fuseau horaire\",\"Description\",\"Devise\",\"Brut \",\"Frais \",\"Net\",\"Solde\",\"Numéro de transaction\",\"Adresse email de l'expéditeur\",\"Nom\",\"Nom de la banque\",\"Compte bancaire\",\"Montant des frais de livraison et de traitement\",\"TVA\",\"Numéro de facture\",\"Numéro de la transaction de référence\"",
                    "\"07/01/2024\",\"08:29:44\",\"Europe/Berlin\",\"EUR\",\"-10,99\",\"0,00\",\"-10,99\",\"-10,99\",\"5678\",\"paypal-se@spotify.com\",\"Spotify AB\",\"\",\"\",\"0,00\",\"0,00\",\"P122\",\"B-1245\"",
                    "\"07/01/2024\",\"08:29:44\",\"Europe/Berlin\",\"Virement bancaire sur le compte PayPal\",\"EUR\",\"10,99\",\"0,00\",\"10,99\",\"0,00\",\"1234\",\"\",\"\",\"Caisse Federale de Banque\",\"7303\",\"0,00\",\"0,00\",\"P122\",\"5678\""
                });

            // WHEN
            var ex = Assert.Throws<BusinessException>(() => importService_sut.ImportPaypalFile("folder/paypal.csv"));

            // THEN
            Assert.That(ex.Message, Is.EqualTo("The line '\"07/01/2024\",\"08:29:44\",\"Europe/Berlin\",\"EUR\",\"-10,99\",\"0,00\",\"-10,99\",\"-10,99\",\"5678\",\"paypal-se@spotify.com\",\"Spotify AB\",\"\",\"\",\"0,00\",\"0,00\",\"P122\",\"B-1245\"' contains 17 fields. Expected 18."));
        }

        [Test]
        public void Should_throw_exception_cannot_parse_transaction_date()
        {
            // GIVEN
            fileSystemService
                .Setup(x => x.ReadAllLines("folder/paypal.csv"))
                .Returns(new List<string>
                {
                    "\"Date\",\"Heure\",\"Fuseau horaire\",\"Description\",\"Devise\",\"Brut \",\"Frais \",\"Net\",\"Solde\",\"Numéro de transaction\",\"Adresse email de l'expéditeur\",\"Nom\",\"Nom de la banque\",\"Compte bancaire\",\"Montant des frais de livraison et de traitement\",\"TVA\",\"Numéro de facture\",\"Numéro de la transaction de référence\"",
                    "\"0701/2024\",\"08:29:44\",\"Europe/Berlin\",\"Paiement préapprouvé d'un utilisateur de facture de paiement\",\"EUR\",\"-10,99\",\"0,00\",\"-10,99\",\"-10,99\",\"5678\",\"paypal-se@spotify.com\",\"Spotify AB\",\"\",\"\",\"0,00\",\"0,00\",\"P122\",\"B-1245\"",
                    "\"07/01/2024\",\"08:29:44\",\"Europe/Berlin\",\"Virement bancaire sur le compte PayPal\",\"EUR\",\"10,99\",\"0,00\",\"10,99\",\"0,00\",\"1234\",\"\",\"\",\"Caisse Federale de Banque\",\"7303\",\"0,00\",\"0,00\",\"P122\",\"5678\""
                });

            // WHEN
            var ex = Assert.Throws<BusinessException>(() => importService_sut.ImportPaypalFile("folder/paypal.csv"));

            // THEN
            Assert.That(ex.Message, Is.EqualTo("The line '\"0701/2024\",\"08:29:44\",\"Europe/Berlin\",\"Paiement préapprouvé d'un utilisateur de facture de paiement\",\"EUR\",\"-10,99\",\"0,00\",\"-10,99\",\"-10,99\",\"5678\",\"paypal-se@spotify.com\",\"Spotify AB\",\"\",\"\",\"0,00\",\"0,00\",\"P122\",\"B-1245\"' could not be parsed: String '0701/2024' was not recognized as a valid DateTime."));
        }

        [TestCase("-20,4a7", "")]
        [TestCase("", "20,4a7")]
        public void Should_throw_exception_cannot_parse_transaction_net(string negative, string positive)
        {
            // GIVEN
            fileSystemService
                .Setup(x => x.ReadAllLines("folder/paypal.csv"))
                .Returns(new List<string>
                {
                    "\"Date\",\"Heure\",\"Fuseau horaire\",\"Description\",\"Devise\",\"Brut \",\"Frais \",\"Net\",\"Solde\",\"Numéro de transaction\",\"Adresse email de l'expéditeur\",\"Nom\",\"Nom de la banque\",\"Compte bancaire\",\"Montant des frais de livraison et de traitement\",\"TVA\",\"Numéro de facture\",\"Numéro de la transaction de référence\"",
                    "\"07/01/2024\",\"08:29:44\",\"Europe/Berlin\",\"Paiement préapprouvé d'un utilisateur de facture de paiement\",\"EUR\",\"-10,99\",\"0,00\",\"-10a,99\",\"-10,99\",\"5678\",\"paypal-se@spotify.com\",\"Spotify AB\",\"\",\"\",\"0,00\",\"0,00\",\"P122\",\"B-1245\"",
                    "\"07/01/2024\",\"08:29:44\",\"Europe/Berlin\",\"Virement bancaire sur le compte PayPal\",\"EUR\",\"10,99\",\"0,00\",\"10,99\",\"0,00\",\"1234\",\"\",\"\",\"Caisse Federale de Banque\",\"7303\",\"0,00\",\"0,00\",\"P122\",\"5678\""
                });

            // WHEN
            var ex = Assert.Throws<BusinessException>(() => importService_sut.ImportPaypalFile("folder/paypal.csv"));

            // THEN
            var faultyString = negative == string.Empty ? positive : negative;
            Assert.That(ex.Message, Is.EqualTo($"The line '\"07/01/2024\",\"08:29:44\",\"Europe/Berlin\",\"Paiement préapprouvé d'un utilisateur de facture de paiement\",\"EUR\",\"-10,99\",\"0,00\",\"-10a,99\",\"-10,99\",\"5678\",\"paypal-se@spotify.com\",\"Spotify AB\",\"\",\"\",\"0,00\",\"0,00\",\"P122\",\"B-1245\"' could not be parsed: The input string '-10a,99' was not in a correct format."));
        }
    }
}
