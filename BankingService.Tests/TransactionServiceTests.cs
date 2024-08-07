﻿using AutoMapper;
using BankingService.Core.API.Interfaces;
using BankingService.Core.API.MapperProfile;
using BankingService.Core.Services;
using BankingService.Core.SPI.Interfaces;
using BankingService.Core.SPI.MapperProfile;
using Moq;

namespace BankingService.Tests
{
    internal class TransactionServiceTests
    {
        Mock<IBankDatabaseService> mockDatabaseService;
        ITransactionService transactionService_sut;

        [SetUp]
        public void SetUp()
        {
            mockDatabaseService = new Mock<IBankDatabaseService>();
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CoreSpiProfile>();
                cfg.AddProfile<CoreApiProfile>();
            }));
            this.transactionService_sut = new TransactionService(mockDatabaseService.Object, mapper);
        }

        [Test]
        public void Should_get_all_transaction()
        {
            // GIVEN
            this.mockDatabaseService
                .Setup(x => x.GetAllTransactions())
                .Returns(new List<Core.SPI.DTOs.TransactionDto>
                {
                    new Core.SPI.DTOs.TransactionDto { Id = 1, Type = "type1", Treasury = 10, AutoComment = "ac1", Category = "cat1", Comment = "cm1", Date = new DateTime(2024,03,26), Flow = 20, Label = "label1" },
                    new Core.SPI.DTOs.TransactionDto { Id = 2, Type = "type2", Treasury = 11, AutoComment = "ac2", Category = "cat2", Comment = "cm2", Date = new DateTime(2024,03,27), Flow = 21, Label = "label2" }
                });

            // WHEN
            var result = this.transactionService_sut.GetAllTransactions();

            // THEN
            CollectionAssert.AreEqual(new List<Core.API.DTOs.TransactionDto>
            {
                new Core.API.DTOs.TransactionDto { Id = 1, Type = "type1", Treasury = 10, AutoComment = "ac1", Category = "cat1", Comment = "cm1", Date = new DateTime(2024,03,26), Flow = 20, Label = "label1" },
                new Core.API.DTOs.TransactionDto { Id = 2, Type = "type2", Treasury = 11, AutoComment = "ac2", Category = "cat2", Comment = "cm2", Date = new DateTime(2024,03,27), Flow = 21, Label = "label2" }
            }, result);
        }

        [Test]
        public void Should_update_transactions()
        {
            // GIVEN
            var transactions = new List<Core.API.DTOs.UpdatableTransactionDto>
            {
                new Core.API.DTOs.UpdatableTransactionDto { Id = 1, Type = "type1", Category = "cat1", Comment = "c1", AutoComment = "ac1" },
                new Core.API.DTOs.UpdatableTransactionDto { Id = 2, Type = "type2", Category = "cat2", Comment = "c2", AutoComment = "ac2" }
            };

            // WHEN
            transactionService_sut.UpdateTransactions(transactions);

            // THEN
            var expected = new List<Core.SPI.DTOs.UpdatableTransactionDto>
            {
                new Core.SPI.DTOs.UpdatableTransactionDto { Id = 1, Type = "type1", Category = "cat1", Comment = "c1", AutoComment = "ac1" },
                new Core.SPI.DTOs.UpdatableTransactionDto { Id = 2, Type = "type2", Category = "cat2", Comment = "c2", AutoComment = "ac2" }
            };
            this.mockDatabaseService.Verify(x => x.UpdateTransactions(It.Is<List<Core.SPI.DTOs.UpdatableTransactionDto>>(actual => actual.IsEqualTo(expected))), Times.Once());
        }

        [Test]
        public void Should_get_transaction_categories_names()
        {
            // GIVEN
            this.mockDatabaseService.Setup(x => x.GetAllCategoriesNames()).Returns(["cat1", "cat2"]);

            // WHEN
            var result = this.transactionService_sut.GetTransactionCategoriesNames();

            // THEN
            CollectionAssert.AreEqual(new List<string> { "cat1", "cat2" }, result);
        }

        [Test]
        public void Should_get_transaction_type_names()
        {
            // GIVEN
            this.mockDatabaseService.Setup(x => x.GetTransactionTypesKvp()).Returns(new Dictionary<string, string> 
            {
                { "key1", "value1" },
                { "key2", "value2" },
                { "key3", "value2" }
            });

            // WHEN
            var result = this.transactionService_sut.GetTransactionTypesNames();

            // THEN
            CollectionAssert.AreEqual(new List<string> { "value1", "value2" }, result);
        }

        [Test]
        public void Should_get_transaction_that_needs_manual_input()
        {
            // GIVEN
            this.mockDatabaseService
                .Setup(x => x.GetTransactionsThatNeedsManualInput())
                .Returns(new List<Core.SPI.DTOs.TransactionDto>
                {
                    new Core.SPI.DTOs.TransactionDto { Id = 1, Type = "type1", Treasury = 10, AutoComment = "ac1", Category = "TODO", Comment = "cm1", Date = new DateTime(2024,03,26), Flow = 20, Label = "label1" },
                    new Core.SPI.DTOs.TransactionDto { Id = 2, Type = "type2", Treasury = 11, AutoComment = "ac2", Category = "TODO", Comment = "cm2", Date = new DateTime(2024,03,27), Flow = 21, Label = "label2" }
                });

            // WHEN
            var result = this.transactionService_sut.GetTransactionsThatNeedsManualInput();

            // THEN
            CollectionAssert.AreEqual(new List<Core.API.DTOs.TransactionDto>
            {
                new Core.API.DTOs.TransactionDto { Id = 1, Type = "type1", Treasury = 10, AutoComment = "ac1", Category = "TODO", Comment = "cm1", Date = new DateTime(2024,03,26), Flow = 20, Label = "label1" },
                new Core.API.DTOs.TransactionDto { Id = 2, Type = "type2", Treasury = 11, AutoComment = "ac2", Category = "TODO", Comment = "cm2", Date = new DateTime(2024,03,27), Flow = 21, Label = "label2" }
            }, result);
        }
    }
}
