using System.ComponentModel.DataAnnotations;

namespace Banking_Website.Models
{
    public class ContactUs
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required, EmailAddress]
        public String Email { get; set; }
        public String Subject { get; set; }
        public String Message { get; set; }

    }
}
