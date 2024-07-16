using AutoMapper;
using BankingService.Core.API.Interfaces;
using BankingService.Core.Exceptions;
using BankingService.Core.Model;
using BankingService.Core.SPI.DTOs;
using BankingService.Core.SPI.Interfaces;
using NLog;
using System.Globalization;

namespace BankingService.Core.Services
{
    public class ImportService : IImportService
    {
        private const string BANK_ARCHIVE_FOLDER = "Archive/Bank_Import";
        private const string PAYPAL_ARCHIVE_FOLDER = "Archive/Paypal_Import";

        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IFileSystemServiceForCore fileSystemService;
        private readonly IBankDatabaseService bankDatabaseService;
        private readonly IMapper mapper;

        public ImportService(IFileSystemServiceForCore fileSystemService, IBankDatabaseService bankDatabaseService, IMapper mapper)
        {
            this.fileSystemService = fileSystemService;
            this.bankDatabaseService = bankDatabaseService;
            this.mapper = mapper;
        }

        public int ImportBankFile(string bankFilePath)
        {
            logger.Info($"Importing {bankFilePath} bank file");
            var csvTransactions = fileSystemService.ReadAllLines(bankFilePath);
            List<Transaction> transactions = GetBankTransactionsFromCSV(csvTransactions);
            ResolveTransactionsAutoFields(transactions);
            int nbImported = bankDatabaseService.InsertTransactionsIfNew(transactions.Select(mapper.Map<TransactionDto>).ToList());
            fileSystemService.ArchiveFile(bankFilePath, BANK_ARCHIVE_FOLDER);
            return nbImported;
        }

        private List<Transaction> GetBankTransactionsFromCSV(List<string> csvTransactions)
        {
            if (csvTransactions.Count == 0 || csvTransactions.FirstOrDefault() != "Date;Date de valeur;Débit;Crédit;Libellé;Solde")
                throw new BusinessException("Not recognized as a bank CSV file.");

            var transactions = new List<Transaction>();
            foreach (var csvTransaction in csvTransactions.Skip(1))
            {
                var splitedTransaction = csvTransaction.Split(";");
                if (splitedTransaction.Length != 6)
                    throw new BusinessException($"The line \"{csvTransaction}\" contains {splitedTransaction.Length} fields. Expected 6.");

                try
                {
                    transactions.Add(new Transaction
                    {
                        Date = DateTime.Parse(splitedTransaction[0]),
                        Flow = GetBankFlow(splitedTransaction),
                        Label = splitedTransaction[4],
                        Treasury = decimal.Parse(splitedTransaction[5], CultureInfo.GetCultureInfo("fr-FR"))
                    });
                }
                catch (Exception ex)
                {
                    throw new BusinessException($"The line \"{csvTransaction}\" could not be parsed: {ex.Message}", ex);
                }
            }

            return transactions;
        }

        private void ResolveTransactionsAutoFields(List<Transaction> transactions)
        {
            var transactionTypes = bankDatabaseService.GetTransactionTypesKvp();
            var transactionCategoriesAndAutoComment = mapper.Map<Dictionary<string, TransactionCategoryAndAutoComment>>(bankDatabaseService.GetTransactionCategoriesAndAutoCommentKvp());

            foreach (var transaction in transactions)
            {
                transaction.ResolveType(transactionTypes);
                transaction.ResolveCategoryAndAutoComment(transactionCategoriesAndAutoComment);
            }

            logger.Info($"{transactions.Count(o => o.Type != "TODO")}/{transactions.Count} transaction types resolved");
            transactions.Where(o => o.Type == "TODO").ToList().ForEach(o => logger.Debug($"Transaction needs a type: '{o.Date};{o.Flow};{o.Treasury};{o.Label}'"));
            logger.Info($"{transactions.Count(o => o.Category != "TODO")}/{transactions.Count} transaction categories resolved");
        }

        private decimal GetBankFlow(string[] splitedTransaction)
        {
            if (string.IsNullOrEmpty(splitedTransaction[2]))
                return decimal.Parse(splitedTransaction[3], CultureInfo.GetCultureInfo("fr-FR"));
            else
                return decimal.Parse(splitedTransaction[2], CultureInfo.GetCultureInfo("fr-FR"));
        }

        public void ImportPaypalFile(string paypalFilePath)
        {
            logger.Info($"Importing {paypalFilePath} paypal file");
            var csvTransactions = fileSystemService.ReadAllLines(paypalFilePath);
            List<Transaction> completeTransactions = MatchPaypalDataToExistingTransactions(csvTransactions);
            try
            {
                bankDatabaseService.UpdateTransactions(completeTransactions.Select(o => mapper.Map<UpdatableTransactionDto>(o.ToUpdatableTransaction())).ToList());
            }
            catch (InvalidOperationException ex)
            {
                throw new BusinessException(ex.Message, ex);
            }
            fileSystemService.ArchiveFile(paypalFilePath, PAYPAL_ARCHIVE_FOLDER);
        }

        private List<Transaction> MatchPaypalDataToExistingTransactions(List<string> csvTransactions)
        {
            var paypalTransactionsQueue = new Queue<PaypalTransaction>(GetPaypalTransactionsFromCSV(csvTransactions).OrderBy(o => o.Date));
            var incompleteTransactions = mapper.Map<List<Transaction>>(bankDatabaseService.GetUnresolvedPaypalTransactions()).OrderBy(o => o.Date).ToList();
            var paypalCategories = bankDatabaseService.GetPaypalCategoriesKvp();

            var completeTransactions = new List<Transaction>();
            while (paypalTransactionsQueue.Count > 0)
            {
                var paypalTransaction = paypalTransactionsQueue.Dequeue();
                var transactionToComplete = incompleteTransactions
                    .FirstOrDefault(o => o.Date == paypalTransaction.OffesetedDate() && o.Flow == paypalTransaction.Net);

                if (transactionToComplete == null)
                {
                    paypalTransaction.DateOffset++;
                    if (paypalTransaction.DateOffset > 31)
                        break;
                    paypalTransactionsQueue.Enqueue(paypalTransaction);
                    continue;
                }

                incompleteTransactions.Remove(transactionToComplete);
                transactionToComplete.AutoComment = paypalTransaction.Nom;
                transactionToComplete.ResolvePaypalCategory(paypalCategories);
                completeTransactions.Add(transactionToComplete);
            }

            logger.Info(paypalTransactionsQueue.Count > 0 ? $"{paypalTransactionsQueue.Count} paypal transactions could not be matched" : "All paypal transactions were matched to data");
            while (paypalTransactionsQueue.Count > 0)
            {
                var unmatchedOp = paypalTransactionsQueue.Dequeue();
                logger.Debug($"Following paypal transaction could not be matched to data: '{unmatchedOp.Date};{unmatchedOp.Net};{unmatchedOp.Nom}'");
            }

            return completeTransactions;
        }

        private List<PaypalTransaction> GetPaypalTransactionsFromCSV(List<string> csvTransactions)
        {
            if (csvTransactions.Count == 0 || csvTransactions.FirstOrDefault() != "\"Date\",\"Heure\",\"Fuseau horaire\",\"Description\",\"Devise\",\"Brut \",\"Frais \",\"Net\",\"Solde\",\"Numéro de transaction\",\"Adresse email de l'expéditeur\",\"Nom\",\"Nom de la banque\",\"Compte bancaire\",\"Montant des frais de livraison et de traitement\",\"TVA\",\"Numéro de facture\",\"Numéro de la transaction de référence\"")
                throw new BusinessException("Not recognized as a paypal CSV file.");

            var transactions = new List<PaypalTransaction>();
            foreach (var transaction in csvTransactions.Skip(1))
            {
                var transactionFeilds = GetCSVFeilds(transaction, ",");
                if (transactionFeilds.Count != 18)
                    throw new BusinessException($"The line '{transaction}' contains {transactionFeilds.Count} fields. Expected 18.");

                PaypalTransaction paypalTransaction;
                try
                {
                    paypalTransaction = new PaypalTransaction()
                    {
                        Date = DateTime.Parse(transactionFeilds[0]),
                        Net = decimal.Parse(transactionFeilds[7], CultureInfo.GetCultureInfo("fr-FR")),
                        Nom = transactionFeilds[11]
                    };
                }
                catch (Exception ex)
                {
                    throw new BusinessException($"The line '{transaction}' could not be parsed: {ex.Message}", ex);
                }
                if (paypalTransaction.Net < 0)
                    transactions.Add(paypalTransaction);
            }

            return transactions;
        }

        private List<string> GetCSVFeilds(string transaction, string delimiter)
        {
            var result = new List<string>();
            string lastIncompleteFeild = string.Empty;

            foreach (var feild in transaction.Split(delimiter))
            {
                if (feild.StartsWith("\"") && feild.EndsWith("\""))
                {
                    result.Add(feild.Replace("\"", ""));
                }
                else if (!feild.StartsWith("\"") && feild.EndsWith("\""))
                {
                    result.Add($"{lastIncompleteFeild},{feild.Replace("\"", "")}");
                }
                else if (feild.StartsWith("\"") && !feild.EndsWith("\""))
                {
                    lastIncompleteFeild = feild.Replace("\"", "");
                }
                else
                {
                    result.Add(feild);
                }
            }

            return result;
        }

        public void RecomputeEveryTransactionAdditionalData()
        {
            logger.Info("Re-computing every transaction additional data");

            var transactions = bankDatabaseService.GetAllTransactions().Select(mapper.Map<Transaction>).ToList();
            var types = bankDatabaseService.GetTransactionTypesKvp();
            var catAndComments = mapper.Map<Dictionary<string, TransactionCategoryAndAutoComment>>(bankDatabaseService.GetTransactionCategoriesAndAutoCommentKvp());
            var paypalCat = bankDatabaseService.GetPaypalCategoriesKvp();

            foreach (var transaction in transactions)
            {
                transaction.ResolveType(types);
                if (transaction.Type == "Paypal")
                    transaction.ResolvePaypalCategory(paypalCat);
                else
                    transaction.ResolveCategoryAndAutoComment(catAndComments);
            }

            logger.Info($"{transactions.Count(o => o.Type == "TODO" || o.Category == "TODO")} transactions still require manual input");

            try
            {
                bankDatabaseService.UpdateTransactions(transactions.Select(o => mapper.Map<UpdatableTransactionDto>(o.ToUpdatableTransaction())).ToList());
            }
            catch (InvalidOperationException ex)
            {
                throw new BusinessException(ex.Message, ex);
            }
        }
    }
}
