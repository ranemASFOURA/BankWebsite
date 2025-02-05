using Banking_Website.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Banking_Website.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly BankDBContext _context;
        private const decimal MaxDepositAmount = 10000; // Max deposit limit
        private const decimal MinBalanceForWithdrawal = 50; // Min balance after withdrawal

        public TransactionsController(BankDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            Accounts account = await _context.Accounts.FirstOrDefaultAsync(a => a.ApplicationUserId == userId);

            if (account == null)
            {
                return NotFound("No account found for the current user.");
            }

            return View(account);
        }



        // GET: Withdraw or Deposit

        [HttpGet]
        public async Task<IActionResult> WithdrawDeposit(int accountId)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null)
            {
                return NotFound(); 
            }

            return View(account); 
        }


        // POST: Withdraw or Deposit
        [HttpPost]
        public async Task<IActionResult> WithdrawDeposit(int accountId, string transactionType, decimal amount)
        {
            if (ModelState.IsValid)
            {
                var account = await _context.Accounts.Include(a => a.Transactions).FirstOrDefaultAsync(a => a.Id == accountId);
                if (account == null)
                {
                    return NotFound();
                }

                if (transactionType == "deposit")
                {
                    if (amount > MaxDepositAmount)
                    {
                        ModelState.AddModelError("", $"Deposit cannot exceed {MaxDepositAmount}.");
                        return View();
                    }

                    account.InitialBalance += amount;

                    // Log the transaction
                    var transaction = new Transactions
                    {
                        AccountId = account.Id,
                        TransactionType = "deposit",
                        Amount = amount,
                        Date = DateTime.Now
                    };
                    _context.Transactions.Add(transaction);
                }
                else if (transactionType == "withdraw")
                {
                    if (account.InitialBalance - amount < MinBalanceForWithdrawal)
                    {
                        ModelState.AddModelError("", $"Insufficient funds. Minimum balance of {MinBalanceForWithdrawal} must remain.");
                        return View();
                    }

                    account.InitialBalance -= amount;

                    // Log the transaction
                    var transaction = new Transactions
                    {
                        AccountId = account.Id,
                        TransactionType = "withdraw",
                        Amount = amount,
                        Date = DateTime.Now
                    };
                    _context.Update(account);
                    _context.Transactions.Add(transaction);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid transaction type.");
                    return View();
                }


                await _context.SaveChangesAsync();
                TempData["Message"] = "successful";
                return RedirectToAction("Index", "Transactions", new { accountId }); // Redirect to a suitable page after transaction
            }
            return View();
        }


        // GET: Transfer
        [HttpGet]
        public async Task<IActionResult> Transfer(int accountId)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        
        // POST: Transfer
        [HttpPost]
        public async Task<IActionResult> Transfer(int accountId, string targetAccountNumber, decimal amount)
        {
            if (ModelState.IsValid)
            {
                var sourceAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
                var targetAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == targetAccountNumber);

                if (sourceAccount == null || targetAccount == null)
                {
                    ModelState.AddModelError("", "Source account or target account not found.");
                    return View();
                }

                if (sourceAccount.InitialBalance - amount < MinBalanceForWithdrawal)
                {
                    ModelState.AddModelError("", $"Insufficient funds. Minimum balance of {MinBalanceForWithdrawal} must remain.");
                    return View();
                }

                sourceAccount.InitialBalance -= amount;
                targetAccount.InitialBalance += amount;

                // Log the transactions
                var sourceTransaction = new Transactions
                {
                    AccountId = sourceAccount.Id,
                    TransactionType = "transfer_out",
                    Amount = amount,
                    targetAccountNumber = targetAccount.AccountNumber,
                    Date = DateTime.Now
                };
                var targetTransaction = new Transactions
                {
                    AccountId = targetAccount.Id,
                    TransactionType = "transfer_in",
                    Amount = amount,
                    Date = DateTime.Now,
                    sourceAccountNumber = sourceAccount.AccountNumber
                };
                _context.Update(sourceAccount);
                _context.Update(targetAccount);
                _context.Transactions.Add(sourceTransaction);
                _context.Transactions.Add(targetTransaction);

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Transactions", new { accountId = sourceAccount.Id }); // Redirect to a suitable page after transfer
            }
            return View();
        }

    }
}
