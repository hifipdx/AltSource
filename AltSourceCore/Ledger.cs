using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace AltSourceCore
{
    public static class Ledger
    {
        private static List<Transaction> transactions;

        public static Transaction ProcessTransaction(int accountID, TransactionType type, decimal amount)
        {
            var newTransaction = default(Transaction);
            var nextID = default(int);
            if (Ledger.transactions == null)
            {
                Ledger.transactions = new List<Transaction>();
                nextID = 1;
            }
            else
            {
                nextID = Ledger.transactions.Max((a) => a.ID) + 1;
            }
            newTransaction = new Transaction
            {
                ID = nextID,
                AccountID = accountID,
                Amount = amount,
                Type = type,
                OccurredAt = DateTime.Now
            };
            Ledger.transactions.Add(newTransaction);
            var previousBalance = AccountManager.GetAccount(accountID).Balance;
            if (type == TransactionType.Credit)
            {
                AccountManager.GetAccount(accountID).Balance = previousBalance + amount;
            }
            else
            {
                AccountManager.GetAccount(accountID).Balance = previousBalance - amount;
            }

            var inProcessPreviousBalance = AccountManager.GetAccount(0).Balance;
            var offsetTransactionType = type == TransactionType.Credit ? TransactionType.Debit : TransactionType.Credit;
            Ledger.transactions.Add(new Transaction
            {
                ID = newTransaction.ID + 1,
                AccountID = 0,
                Amount = amount,
                Type = offsetTransactionType,
                OccurredAt = DateTime.Now,
                RelatedID = newTransaction.ID
            });
            if (offsetTransactionType == TransactionType.Credit)
            {
                AccountManager.GetAccount(0).Balance = inProcessPreviousBalance + amount;
            }
            else
            {
                AccountManager.GetAccount(0).Balance = inProcessPreviousBalance - amount;
            }
            return newTransaction;
        }

        public static IEnumerable<Transaction> GetTransactions(string accountName)
        {
            var transactionList = default(IEnumerable<Transaction>);
            if (!String.IsNullOrWhiteSpace(accountName))
            {
                var account = AccountManager.GetAccount(accountName);
                if (account != null)
                {
                    transactionList = GetTransactions(account.ID);
                }
            }
            return transactionList;
        }

        public static IEnumerable<Transaction> GetTransactions(int accountID)
        {
            var transactionList = default(IEnumerable<Transaction>);
            if (Ledger.transactions != null)
            {
                var transactionQuery = Ledger.transactions.Where((a) => a.AccountID == accountID);
                if (transactionQuery != null & transactionQuery.Any())
                {
                    transactionList = new ReadOnlyCollection<Transaction>(transactionQuery.ToList());
                }
            }
            return transactionList;
        }
    }
}
