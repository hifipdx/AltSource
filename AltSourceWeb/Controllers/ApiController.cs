using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltSourceCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AltSourceWeb.Controllers
{
    [Route("api")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult LogIn(LogIn login)
        {
            var result = default(IActionResult);
            if (login == null || String.IsNullOrWhiteSpace(login.UserID) || String.IsNullOrWhiteSpace(login.Password))
            {
                result = BadRequest();
            }
            else
            {
                var user = UserProfileManager.GetProfile(login.UserID);
                if (user == null)
                {
                    result = NotFound("Invalid User ID or password");
                }
                else
                {
                    if (UserProfileManager.IsPasswordValid(login.UserID, login.Password))
                    {
                        result = Ok();
                    }
                    else
                    {
                        result = NotFound("Invalid User ID or password");
                    }
                }
            }
            return result;
        }

        [HttpPost("register")]
        public IActionResult Register(LogIn login)
        {
            var result = default(IActionResult);
            if (login == null || String.IsNullOrWhiteSpace(login.UserID) || String.IsNullOrWhiteSpace(login.Password))
            {
                result = BadRequest();
            }
            else
            {
                try
                {
                    UserProfileManager.AddProfile(login.UserID, login.Password);
                    result = Ok();
                }
                catch (ApplicationException ex)
                {
                    result = BadRequest(ex.Message);
                }
                catch (Exception)
                {
                    result = BadRequest("An error occurred");
                }
            }
            return result;
        }

        [HttpGet("accounts/{ID:int?}")]
        public IActionResult GetAccount(int ID = 0, string ownerID = null)
        {
            var result = default(IActionResult);
            if (ID == 0)
            {
                if (!String.IsNullOrWhiteSpace(ownerID))
                {
                    var accounts = AccountManager.GetAccounts(ownerID);
                    if (accounts != null)
                    {
                        result = Ok(accounts);
                    }
                }
                else
                {
                    result = Ok(new Account());
                }
            }
            else
            {
                var account = AccountManager.GetAccount(ID);
                if (account == null)
                {
                    result = NotFound();
                }
                else
                {
                    result = Ok(account);
                }
            }
            return result;
        }

        [HttpPost("accounts")]
        public IActionResult CreateAccount(Account account)
        {
            var result = default(IActionResult);
            if (account == null || String.IsNullOrWhiteSpace(account.Name) || String.IsNullOrWhiteSpace(account.OwnerUserID))
            {
                result = BadRequest();
            }
            else
            {
                try
                {
                    var newAccount  = AccountManager.AddAccount(account.OwnerUserID, account.Name);
                    if (newAccount == null)
                    {
                        result = BadRequest("Add account failed");
                    }
                    else
                    {
                        result = Ok(newAccount);
                    }
                }
                catch (ApplicationException ex)
                {
                    result = BadRequest(ex.Message);
                }
                catch (Exception)
                {
                    result = BadRequest("An error occurred");
                }
            }
            return result;
        }

        [HttpGet("transactions/{ID:int?}")]
        public IActionResult GetTransaction(int ID = 0, int accountID = 0)
        {
            var result = default(IActionResult);
            if (ID == 0)
            {
                if (accountID == 0)
                {
                    result = Ok(new Transaction());
                }
                else
                {
                    var transactions = Ledger.GetTransactions(accountID);
                    result = Ok(transactions);
                }
            }
            else
            {
            }
            return result;
        }

        [HttpPost("transactions")]
        public IActionResult ProcessTransaction(Transaction transaction)
        {
            var result = default(IActionResult);
            var newTransaction = Ledger.ProcessTransaction(transaction.AccountID, transaction.Type, transaction.Amount);
            if (newTransaction == null)
            {
                result = BadRequest("Process deposit failed");
            }
            else
            {
                result = Ok(newTransaction);
            }
            return result;
        }

    }



    public class LogIn
    {
        public string UserID { get; set; }
        public string Password { get; set; }
    }

}