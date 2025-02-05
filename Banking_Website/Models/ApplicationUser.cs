using Microsoft.AspNetCore.Identity;

namespace Banking_Website.Models
{
    public class ApplicationUser : IdentityUser

    {
        public string Name { get; set; }
        public string address { get; set; }
        public Accounts Account { get; set; }
    }
}
