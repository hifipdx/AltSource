using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace AltSourceCore
{
    public static class AccountManager
    {
        private static List<Account> accounts;

        static AccountManager()
        {
            AccountManager.accounts = new List<Account>();
            var inProcessAccount = (new Account
            {
                ID = 0,
                OwnerUserID = "system",
                Name = "InProcess",
                Balance = 1000000
            });
            AccountManager.accounts.Add(inProcessAccount);
        }

        public static Account AddAccount(string userID, string accountName)
        {
            var newAccount = default(Account);
            if (!String.IsNullOrWhiteSpace(userID) && !String.IsNullOrWhiteSpace(accountName))
            {
                if (GetAccount(accountName) != null)
                {
                    throw new ApplicationException(String.Format("Account name {0} already exists", accountName));
                }
                var nextID = AccountManager.accounts.Max((a) => a.ID) + 1;
                newAccount = new Account
                {
                    ID = nextID,
                    OwnerUserID = userID,
                    Name = accountName
                };
                AccountManager.accounts.Add(newAccount);
            }
            return newAccount;
        }

        public static Account GetAccount(int ID)
        {
            var account = default(Account);
            if (AccountManager.accounts != null)
            {
                account = accounts.FirstOrDefault(a => a.ID == ID);
            }
            return account;
        }

        public static Account GetAccount(string accountName)
        {
            var account = default(Account);
            if (AccountManager.accounts != null && !String.IsNullOrWhiteSpace(accountName))
            {
                account = accounts.FirstOrDefault(a => a.Name.Equals(accountName, StringComparison.InvariantCultureIgnoreCase));
            }
            return account;
        }

        public static IEnumerable<Account> GetAccounts(string userID)
        {
            var accountList = default(IEnumerable<Account>);
            if (AccountManager.accounts != null && !String.IsNullOrWhiteSpace(userID))
            {
                var accountQuery = accounts.Where(a => a.OwnerUserID.Equals(userID, StringComparison.InvariantCultureIgnoreCase));
                if (accountQuery != null && accountQuery.Any())
                {
                    accountList = new ReadOnlyCollection<Account>(accountQuery.ToList());
                }
            }
            return accountList;
        }
    }
}
