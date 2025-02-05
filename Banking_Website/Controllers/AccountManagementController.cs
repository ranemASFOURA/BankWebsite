using Banking_Website.Models;
using Banking_Website.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace Banking_Website.Controllers
{

    [Authorize]
    public class AccountManagementController : Controller
    {
        private readonly BankDBContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        public AccountManagementController(BankDBContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // GET: CreateAccount
        [HttpGet]
        public IActionResult CreateAccount()
        {
            return View();
        }

        // POST: CreateAccount
        [HttpPost]
        public async Task<IActionResult> CreateAccount(CreateAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data. Please check your inputs!";
                // Return the view with the model to display validation errors
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            // Check if the account already exists
            var existingAccount = await _context.Accounts
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.Customer.Email == model.Email);

            if (existingAccount != null)
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                TempData["ErrorMessage"] = "This email is already registered!";
                return View(model); // Return the model with errors
            }

            // Validate initial balance
            if (model.InitialBalance.HasValue && model.InitialBalance < 0) // Assuming minimum balance is 100
            {
                ModelState.AddModelError("InitialBalance", "Initial balance must be a positive value.");
                return View(model); // Return the model with errors
            }

            // Create a new account
            var newAccount = new Accounts
            {
                ApplicationUserId = userId,
                InitialBalance = model.InitialBalance ?? 0, // Default to 0 if no balance is provided
                SecurityQuestion = model.SecurityQuestion,
                SecurityAnswer = model.SecurityAnswer
            };

            _context.Accounts.Add(newAccount);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Bank account created successfully!";
            return RedirectToAction("Index", "Transactions", new { accountId = newAccount.Id });
        }



        [HttpGet]
        public IActionResult AccountDetails(int accountId)
        {
            var account = _context.Accounts.Include(a => a.Customer).FirstOrDefault(a => a.Id == accountId);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        
        [HttpPost]
        public async Task<IActionResult> EditAccount(int accountId, decimal newBalance, string newSecurityQuestion, string newSecurityAnswer)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null)
            {
                TempData["ErrorMessage"] = "Account not found!";
                return RedirectToAction("Index");
            }

                account.InitialBalance = newBalance;
            account.SecurityQuestion = newSecurityQuestion;
            account.SecurityAnswer = newSecurityAnswer;

            _context.Update(account);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Account details updated successfully!";
            return RedirectToAction("AccountDetails", new { accountId = accountId });
        }


        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Find the account linked to the current user
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.ApplicationUserId == userId);

            if (account == null)
            {
                return NotFound("No account found for the current user.");
            }

            // Remove the account from the database
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Your account has been deleted successfully.";

            // Redirect to the index page after deletion
            return RedirectToAction("Index", "AccountManagement");
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.ApplicationUserId == userId);

            if (account == null)
            {
                return NotFound("No account found for the current user.");
            }

            var transactions = await _context.Transactions
                .Where(t => t.AccountId == account.Id)
                .Select(t => new Transactions
                {
                    Id = t.Id,
                    TransactionType = t.TransactionType,
                    Amount = t.Amount,
                    Date = t.Date,
                    targetAccountNumber = t.targetAccountNumber,
                    sourceAccountNumber = account.AccountNumber
                })
                .ToListAsync();

            return View(transactions);
        }


    }
}

