using AltSourceCore;
using System;
using System.Linq;

/// <summary>
/// AltSource Ledger 
/// Created by Tony Thompson
/// September/October, 2019
/// </summary>

namespace AltSourceConsole
{
    class LedgerConsole
    {

        private static string loggedInAsUserID;

        static void Main(string[] args)
        {
            DisplayLoginScreen();
        }

        static void DisplayLoginScreen()
        {
            var userResponse = default(UserResponse<int>);
            var returnMessage = default(string);
            do
            {
                Console.Clear();
                Console.WriteLine("Welcome to AltSource Ledger");
                Console.WriteLine(Environment.NewLine);
                if (!String.IsNullOrWhiteSpace(returnMessage))
                {
                    Console.WriteLine(String.Format("*** {0} ***", returnMessage));
                    Console.WriteLine(Environment.NewLine);
                    returnMessage = null;
                }
                Console.WriteLine("1 - Log In");
                Console.WriteLine("2 - Register new user");
                userResponse = GetUserResponse<int>("Please enter your selection", 3);
                if (!userResponse.IsExit && userResponse.IsValid)
                {
                    switch (userResponse.Value)
                    {
                        case 1:
                            returnMessage = LogIn();
                            break;
                        case 2:
                            returnMessage = RegisterNewUser();
                            break;
                    }
                }
            } while (userResponse != null && !userResponse.IsExit);
        }


        static string LogIn()
        {
            var returnMessage = default(string);
            var userID = default(string);
            var password = default(string);
            LedgerConsole.loggedInAsUserID = null;
            Console.Clear();
            Console.WriteLine("AltSource Ledger - Log In");
            var userIDResponse = GetUserResponse<string>("Please enter your user ID", 3);
            if (!userIDResponse.IsExit && userIDResponse.IsValid)
            {
                userID = userIDResponse.Value;
                var userProfile = UserProfileManager.GetProfile(userID);
                var userProfileExists = false;
                var passwordValid = false;
                if (userProfile == null)
                {
                    var passwordResponse = GetUserResponse<string>("Please enter your password", 3);
                }
                else
                {
                    userProfileExists = true;
                    var passwordResponse = GetUserResponse<string>("Please enter your password", 3);
                    if (!passwordResponse.IsExit && passwordResponse.IsValid)
                    {
                        password = passwordResponse.Value;
                        if (UserProfileManager.IsPasswordValid(userID, password))
                        {
                            passwordValid = true;
                        }
                    }
                }
                if (userProfileExists && passwordValid)
                {
                    LedgerConsole.loggedInAsUserID = userID;
                    DisplayMainMenu();
                }
                else
                {
                    returnMessage = "Invalid user ID or password";
                }
            }
            return returnMessage;
        }

        static string RegisterNewUser()
        {
            var returnMessage = default(string);
            var userID = default(string);
            var password = default(string);

            Console.Clear();
            Console.WriteLine("AltSource Ledger - Register New User");
            var userIDResponse = GetUserResponse<string>("Please enter a new user ID", 3);
            if (!userIDResponse.IsExit && userIDResponse.IsValid)
            {
                userID = userIDResponse.Value;
                var passwordResponse = GetUserResponse<string>("Please enter a password", 3);
                if (!passwordResponse.IsExit && passwordResponse.IsValid)
                {
                    password = passwordResponse.Value;
                    passwordResponse = GetUserResponse<string>("Please re-enter the password", 3);
                    if (!passwordResponse.IsExit && passwordResponse.IsValid)
                    {
                        if (passwordResponse.Value != password)
                        {
                            Console.WriteLine("Passwords do not match. Press Enter to return to the Log In screen.");
                            Console.ReadLine();
                            returnMessage = "New user registration canceled";
                        }
                        else
                        {
                            try
                            {
                                UserProfileManager.AddProfile(userID, password);
                                returnMessage = String.Format("Successfully registered user ID '{0}'", userID);
                            }
                            catch (ApplicationException ex)
                            {
                                returnMessage = ex.Message;
                            }
                            catch (Exception ex)
                            {
                                returnMessage = "An error occurred while registering new user";
                            }
                        }
                    }
                }
            }
            return returnMessage;
        }

        static string DisplayMainMenu()
        {
            var userResponse = default(UserResponse<int>);
            var returnMessage = default(string);
            do
            {
                Console.Clear();
                Console.WriteLine("AltSource Ledger - Main Menu");
                Console.WriteLine(Environment.NewLine);
                if (!String.IsNullOrWhiteSpace(returnMessage))
                {
                    Console.WriteLine(String.Format("*** {0} ***", returnMessage));
                    Console.WriteLine(Environment.NewLine);
                    returnMessage = null;
                }
                Console.WriteLine("1 - Create an account");
                Console.WriteLine("2 - Record a deposit");
                Console.WriteLine("3 - Record a withdrawal");
                Console.WriteLine("4 - View account balance");
                Console.WriteLine("5 - View transaction history");
                Console.WriteLine("6 - Log Out");
                userResponse = GetUserResponse<int>("Please enter your selection", 3);
                if (!userResponse.IsExit && userResponse.IsValid)
                {
                    switch (userResponse.Value)
                    {
                        case 1:
                            returnMessage = CreateAccount();
                            break;
                        case 2:
                            returnMessage = ProcessDeposit();
                            break;
                        case 3:
                            returnMessage = ProcessWithdrawal();
                            break;
                        case 4:
                            returnMessage = ViewAccountBalance();
                            break;
                        case 5:
                            returnMessage = ViewTransactionHistory();
                            break;
                        case 6:
                            LedgerConsole.loggedInAsUserID = null;
                            userResponse.IsExit = true;
                            break;
                    }
                }
            } while (userResponse != null && !userResponse.IsExit);
            return returnMessage;
        }

        static string CreateAccount()
        {
            var returnMessage = default(string);
            Console.Clear();
            Console.WriteLine("AltSource Ledger - Create Account");
            var accountName = default(string);
            var accountNameResponse = GetUserResponse<string>("Please enter an account name", 3);
            if (!accountNameResponse.IsExit && accountNameResponse.IsValid)
            {
                accountName = accountNameResponse.Value;
                var newAccount = default(Account);
                try
                {
                    newAccount = AccountManager.AddAccount(LedgerConsole.loggedInAsUserID, accountName);
                    if (newAccount != null)
                    {
                        returnMessage = String.Format("Successfully added new account '{0}'", accountName);
                    }
                }
                catch (ApplicationException ex)
                {
                    returnMessage = ex.Message;
                }
                catch (Exception ex)
                {
                    returnMessage = "An error occurred while creating account";
                }
            }
            return returnMessage;
        }

        static string ProcessDeposit()
        {
            var returnMessage = default(string);
            Console.Clear();
            Console.WriteLine("AltSource Ledger - Record Deposit");
            var accountMenuResponse = DisplayAccountMenu();
            if (!accountMenuResponse.IsExit && accountMenuResponse.IsValid)
            {
                var accountID = accountMenuResponse.Value;
                var transactionAmount = default(decimal);
                var transactionAmountResponse = GetUserResponse<decimal>("Please enter the deposit amount", 3);
                if (!transactionAmountResponse.IsExit && transactionAmountResponse.IsValid)
                {
                    transactionAmount = transactionAmountResponse.Value;
                    var transaction = Ledger.ProcessTransaction(accountID, TransactionType.Credit, transactionAmount);
                    if (transaction == null)
                    {
                        returnMessage = "Deposit transaction did not succeed";
                    }
                    else
                    {
                        returnMessage = String.Format("Deposit transaction ID {0} was processed successfully", transaction.ID);
                    }
                }
            }
            return returnMessage;
        }

        static string ProcessWithdrawal()
        {
            var returnMessage = default(string);
            Console.Clear();
            Console.WriteLine("AltSource Ledger - Record Withdrawal");
            var accountMenuResponse = DisplayAccountMenu();
            if (!accountMenuResponse.IsExit && accountMenuResponse.IsValid)
            {
                var accountID = accountMenuResponse.Value;
                var transactionAmount = default(decimal);
                var transactionAmountResponse = GetUserResponse<decimal>("Please enter the withdrawal amount", 3);
                if (!transactionAmountResponse.IsExit && transactionAmountResponse.IsValid)
                {
                    transactionAmount = transactionAmountResponse.Value;
                    var transaction = Ledger.ProcessTransaction(accountID, TransactionType.Debit, transactionAmount);
                    if (transaction == null)
                    {
                        returnMessage = "Withdrawal transaction did not succeed";
                    }
                    else
                    {
                        returnMessage = String.Format("Withdrawal transaction ID {0} was processed successfully", transaction.ID);
                    }
                }
            }
            return returnMessage;
        }

        static string ViewTransactionHistory()
        {
            var returnMessage = default(string);
            Console.Clear();
            Console.WriteLine("AltSource Ledger - View Transaction History");

            var accountID = default(int);
            var accountMenuResponse = DisplayAccountMenu();
            if (!accountMenuResponse.IsExit && accountMenuResponse.IsValid)
            {
                accountID = accountMenuResponse.Value;
                var transactionList = Ledger.GetTransactions(accountID);
                if (transactionList == null || !transactionList.Any())
                {
                    Console.WriteLine("No transactions found. Press Enter to return to the Main Menu.");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine(Environment.NewLine);
                    transactionList.ToList().ForEach((a) =>
                        Console.WriteLine("{0} in the amount of {1:C} on {2}", a.Type, a.Amount, a.OccurredAt)
                    ); ;
                    Console.WriteLine(Environment.NewLine);
                    Console.WriteLine("Press Enter to return to the Main Menu.");
                    Console.ReadLine();
                }
            }
            return returnMessage;
        }

        static string ViewAccountBalance()
        {
            var returnMessage = default(string);
            Console.Clear();
            Console.WriteLine("AltSource Ledger - View Account Balance");
            var accountMenuResponse = DisplayAccountMenu();
            if (!accountMenuResponse.IsExit && accountMenuResponse.IsValid)
            {
                var accountID = accountMenuResponse.Value;
                var account = AccountManager.GetAccount(accountID);
                if (account != null)
                {
                    Console.WriteLine(Environment.NewLine);
                    Console.WriteLine("'{0}' account balance is {1:C}", account.Name, account.Balance);
                    Console.WriteLine(Environment.NewLine);
                    Console.WriteLine("Press Enter to return to the Main Menu.");
                    Console.ReadLine();
                }
            }
            return returnMessage;
        }

        static UserResponse<int> DisplayAccountMenu()
        {
            var selection = default(UserResponse<int>);
            var accountList = AccountManager.GetAccounts(LedgerConsole.loggedInAsUserID);
            if (accountList == null || !accountList.Any())
            {
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine(String.Format("No accounts found for user ID '{0}'.  Press Enter to return.", LedgerConsole.loggedInAsUserID));
                Console.ReadLine();
                selection = new UserResponse<int> { IsExit = true };
            }
            else
            {
                Console.WriteLine(Environment.NewLine);
                accountList.ToList().ForEach((a) =>
                    Console.WriteLine("{0} - {1}", a.ID, a.Name)
                );
                selection = GetUserResponse<int>("Please select one of the accounts above", 3);
            }
            return selection;
        }

        static UserResponse<T> GetUserResponse<T>(string promptText, int maxRetries)
        {
            var finalResponse = new UserResponse<T>();
            var tryCounter = 0;
            var response = default(UserResponse<T>);
            do
            {
                response = GetUserResponse<T>(promptText);
                if (!response.IsExit && !response.IsValid)
                {
                    Console.WriteLine("Invalid response");
                    tryCounter++;
                }
            } while (!response.IsExit && !response.IsValid && tryCounter < maxRetries);
            finalResponse = response;
            return finalResponse;
        }

        static UserResponse<T> GetUserResponse<T>(string promptText)
        {
            var response = new UserResponse<T>();
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(String.Format("{0} or X to exit:", promptText));
            var rawResponse = Console.ReadLine();
            if (rawResponse.Equals("x", StringComparison.InvariantCultureIgnoreCase))
            {
                response.IsExit = true;
            }
            else
            {
                if (typeof(T) == typeof(string))
                {
                    if (!String.IsNullOrEmpty(rawResponse))
                    {
                        response.Value = (T)Convert.ChangeType(rawResponse, typeof(T));
                        response.IsValid = true;
                    }
                }
                else if (typeof(T) == typeof(int))
                {
                    var intValue = default(int);
                    if (int.TryParse(rawResponse, out intValue))
                    {
                        response.Value = (T)Convert.ChangeType(rawResponse, typeof(T));
                        response.IsValid = true;
                    }
                }
                else if (typeof(T) == typeof(decimal))
                {
                    var decimalValue = default(decimal);
                    if (decimal.TryParse(rawResponse, out decimalValue))
                    {
                        response.Value = (T)Convert.ChangeType(rawResponse, typeof(T));
                        response.IsValid = true;
                    }
                }
                else if (typeof(T) == typeof(bool))
                {
                    if (!String.IsNullOrEmpty(rawResponse))
                    {
                        if (rawResponse.Equals("y", StringComparison.InvariantCultureIgnoreCase))
                        {
                            response.Value = (T)Convert.ChangeType(true, typeof(T));
                            response.IsValid = true;
                        }
                        else if (rawResponse.Equals("n", StringComparison.InvariantCultureIgnoreCase))
                        {
                            response.Value = (T)Convert.ChangeType(false, typeof(T));
                            response.IsValid = true;
                        }
                    }
                }
            }
            return response;
        }
    }

    class UserResponse<T>
    {
        public bool IsExit { get; set; }
        public bool IsValid { get; set; }
        public T Value { get; set; }
    }
}
