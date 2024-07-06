using BankingService.Core.API.Interfaces;
using BankingService.Core.Services;
using BankingService.Core.SPI.DTOs;
using BankingService.Core.SPI.Interfaces;
using BankingService.Infra.Database.Services;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingService.Tests.ImportServiceTests
{
    internal class CategorieRecomputeTests
    {
        Mock<IFileSystemServiceForCore> fileSystemService;
        Mock<IBankDatabaseService> bankDatabaseService;
        IImportService importService_sut;

        [SetUp]
        public void Setup()
        {
            fileSystemService = new Mock<IFileSystemServiceForCore>();
            bankDatabaseService = new Mock<IBankDatabaseService>();
            importService_sut = new ImportService(fileSystemService.Object, bankDatabaseService.Object);
        }

        [Test]
        public void Should_left_untouched_a_line_whose_data_does_not_have_to_change()
        {
            GenericRecomputeTestCode(
                new TransactionDto { Id = 1, Date = new DateTime(2024, 10, 20), Flow = 1m, Treasury = 2m, Label = "PSC AUCHAN", Type = "Sans Contact", Category = "Nourriture", AutoComment = "Courses (Auchan)", Comment = "aaa" },
                new UpdatableTransactionDto { Id = 1, Type = "Sans Contact", Category = "Nourriture", AutoComment = "Courses (Auchan)", Comment = "aaa" }
                );
        }

        [Test]
        public void Should_fill_incomplete_type_category_and_autocomment()
        {
            GenericRecomputeTestCode(
                new TransactionDto { Id = 1, Date = new DateTime(2024, 10, 21), Flow = 1m, Treasury = 3m, Label = "PSC AUCHAN", Type = "TODO", Category = "TODO", AutoComment = "", Comment = "bbb" },
                new UpdatableTransactionDto { Id = 1, Type = "Sans Contact", Category = "Nourriture", AutoComment = "Courses (Auchan)", Comment = "bbb" }
                );
        }

        [Test]
        public void Should_update_type_category_and_autocomment_because_they_changed()
        {
            GenericRecomputeTestCode(
                new TransactionDto { Id = 1, Date = new DateTime(2024, 10, 22), Flow = 1m, Treasury = 4m, Label = "PSC AUCHAN", Type = "a", Category = "b", AutoComment = "c", Comment = "ccc" },
                new UpdatableTransactionDto { Id = 1, Type = "Sans Contact", Category = "Nourriture", AutoComment = "Courses (Auchan)", Comment = "ccc" }
                );
        }

        [Test]
        public void Should_leave_alone_lines_that_still_cannot_be_resolved()
        {
            GenericRecomputeTestCode(
                new TransactionDto { Id = 1, Date = new DateTime(2024, 10, 23), Flow = 1m, Treasury = 5m, Label = "AAA", Type = "TODO", Category = "TODO", AutoComment = "", Comment = "ddd" },
                new UpdatableTransactionDto { Id = 1, Type = "TODO", Category = "TODO", AutoComment = "", Comment = "ddd" }
                );
        }

        [Test]
        public void Should_leave_type_untouched_and_not_lose_other_data()
        {
            GenericRecomputeTestCode(
                new TransactionDto { Id = 1, Date = new DateTime(2024, 10, 24), Flow = 1m, Treasury = 6m, Label = "PSC BBB", Type = "Sans Contact", Category = "Special", AutoComment = "", Comment = "eee" },
                new UpdatableTransactionDto { Id = 1, Type = "Sans Contact", Category = "Special", AutoComment = "", Comment = "eee" }
                );
        }

        [Test]
        public void Should_only_update_the_type_and_not_lose_other_data()
        {
            GenericRecomputeTestCode(
                new TransactionDto { Id = 1, Date = new DateTime(2024, 10, 25), Flow = 1m, Treasury = 7m, Label = "PSC CCC", Type = "Virement", Category = "Special 2", AutoComment = "", Comment = "fff" },
                new UpdatableTransactionDto { Id = 1, Type = "Sans Contact", Category = "Special 2", AutoComment = "", Comment = "fff" }
                );
        }

        [Test]
        public void Should_update_paypal_categorie()
        {
            GenericRecomputeTestCode(
                new TransactionDto { Id = 1, Date = new DateTime(2024, 10, 26), Flow = 1m, Treasury = 8m, Label = "PAYPAL", Type = "Paypal", Category = "TODO", AutoComment = "Spotify AB", Comment = "ggg" },
                new UpdatableTransactionDto { Id = 1, Type = "Paypal", Category = "Loisirs", AutoComment = "Spotify AB", Comment = "ggg" }
                );
        }

        [Test]
        public void Should_not_update_paypal_categorie_when_there_is_no_new_match()
        {
            GenericRecomputeTestCode(
                new TransactionDto { Id = 1, Date = new DateTime(2024, 10, 27), Flow = 1m, Treasury = 9m, Label = "PAYPAL", Type = "Paypal", Category = "TODO", AutoComment = "nomatch", Comment = "hhh" },
                new UpdatableTransactionDto { Id = 1, Type = "Paypal", Category = "TODO", AutoComment = "nomatch", Comment = "hhh" }
                );
        }

        [Test]
        public void Should_not_be_bothered_by_an_empty_paypal_auto_comment()
        {
            GenericRecomputeTestCode(
                new TransactionDto { Id = 1, Date = new DateTime(2024, 10, 28), Flow = 1m, Treasury = 10m, Label = "PAYPAL", Type = "Paypal", Category = "TODO", AutoComment = "", Comment = "iii" },
                new UpdatableTransactionDto { Id = 1, Type = "Paypal", Category = "TODO", AutoComment = "", Comment = "iii" }
                );
        }

        [Test]
        public void Should_override_manually_categorized_data_when_a_new_match_is_found()
        {
            GenericRecomputeTestCode(
                new TransactionDto { Id = 1, Date = new DateTime(2024, 10, 29), Flow = 1m, Treasury = 11m, Label = "PSC NEW", Type = "Sans Contact", Category = "Manually categorized", AutoComment = "", Comment = "jjj" },
                new UpdatableTransactionDto { Id = 1, Type = "Sans Contact", Category = "NewCat", AutoComment = "NewComment", Comment = "jjj" }
                );
        }

        [Test]
        public void Should_override_manually_categorized_paypal_data_when_a_new_match_is_found()
        {
            GenericRecomputeTestCode(
                new TransactionDto { Id = 1, Date = new DateTime(2024, 10, 30), Flow = 1m, Treasury = 12m, Label = "PAYPAL", Type = "Paypal", Category = "Manually categorized paypal", AutoComment = "newMatch", Comment = "kkk" },
                new UpdatableTransactionDto { Id = 1, Type = "Paypal", Category = "NewPaypalCat", AutoComment = "newMatch", Comment = "kkk" }
                );
        }

        public void GenericRecomputeTestCode(TransactionDto input, UpdatableTransactionDto expected)
        {
            // GIVEN
            bankDatabaseService.Setup(x => x.GetAllTransactions()).Returns([input]);
            bankDatabaseService.Setup(x => x.GetTransactionTypesKvp()).Returns(new Dictionary<string, string>
            {
                { "PSC", "Sans Contact" },
                { "PAYPAL", "Paypal" }
            });
            bankDatabaseService.Setup(x => x.GetTransactionCategoriesAndAutoCommentKvp()).Returns(new Dictionary<string, TransactionCategoryAndAutoCommentDto>
            {
                { "AUCHAN", new TransactionCategoryAndAutoCommentDto { Category = "Nourriture", AutoComment = "Courses (Auchan)" } },
                { "NEW", new TransactionCategoryAndAutoCommentDto { Category = "NewCat", AutoComment = "NewComment" } }
            });
            bankDatabaseService.Setup(x => x.GetPaypalCategoriesKvp()).Returns(new Dictionary<string, string>
            {
                { "Spotify", "Loisirs" },
                { "newMatch", "NewPaypalCat" }
            });

            // WHEN
            importService_sut.RecomputeEveryTransactionAdditionalData();

            // THEN
            bankDatabaseService.Verify(x => x.UpdateTransactions(It.Is<List<UpdatableTransactionDto>>(actual => TestHelpers.CheckUpdatableTransactionDtos(actual, new List<UpdatableTransactionDto> { expected }))), Times.Once());
        }
    }
}
