using Banking_Website.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace Banking_Website.Controllers
{
    public class HomeController : Controller
    {

       BankDBContext db;
        public HomeController(BankDBContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SaveContact(ContactUs model)
        {
            if (ModelState.IsValid)
            {
                db.ContactUs.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index","Home");
            }
            return View("Contact", model);
        }
        public IActionResult Services()
        {
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}