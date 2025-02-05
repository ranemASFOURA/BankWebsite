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

        // GET: CreateAccount
        [HttpGet]
        public IActionResult CreateAccount()
        {
            return View();
        }

        // POST: CreateAccount
        [HttpPost]
        public async Task<IActionResult> CreateAccount(string name, string email, decimal initialBalance, string securityQuestion, string securityAnswer)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (ModelState.IsValid)
            {
                // Check if the account already exists
                var existingAccount = await _context.Accounts
                    .Include(a => a.Customer)
                    .FirstOrDefaultAsync(a => a.Customer.Email == email);

                if (existingAccount != null)
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    return View();
                }

                // Create a new account
                var newAccount = new Accounts
                {
                    ApplicationUserId = userId,
                    InitialBalance = initialBalance,
                    SecurityQuestion = securityQuestion,
                    SecurityAnswer = securityAnswer
                };

                _context.Accounts.Add(newAccount);
                await _context.SaveChangesAsync();

                
                return RedirectToAction("Index", "Transactions", new { accountId = newAccount.Id });
            }
            return View();
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
                return NotFound();
            }

            account.InitialBalance = newBalance;
            account.SecurityQuestion = newSecurityQuestion;
            account.SecurityAnswer = newSecurityAnswer;

            _context.Update(account);
            await _context.SaveChangesAsync();
            return RedirectToAction("AccountDetails", new { accountId = accountId });
        }

        // حذف الحساب
        [HttpPost]
        public async Task<IActionResult> DeleteAccount(int accountId)
        {
            var account = await _context.Accounts.Include(a => a.Customer).FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null)
            {
                return NotFound();
            }

            // حذف الحساب
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
        
    }
}

