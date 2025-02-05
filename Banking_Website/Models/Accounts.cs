using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Banking_Website.Models
{
    public class Accounts
    {
        public int Id { get; set; }
       
        public string ApplicationUserId { get; set; }

        
        public ApplicationUser Customer { get; set; }

        // AccountNumber is now generated automatically
        [Required(ErrorMessage = "Account number is required.")]
        [StringLength(11, ErrorMessage = "Account number must be 11 digits long.", MinimumLength = 11)]
        public string AccountNumber { get; private set; }

        [Required(ErrorMessage = "Initial balance is required.")]
        [Range(100, double.MaxValue, ErrorMessage = "Initial balance must be at least 100.")]
        public decimal InitialBalance { get; set; }

        [Required(ErrorMessage = "Security question is required.")]
        [StringLength(100, ErrorMessage = "Security question cannot exceed 100 characters.")]
        public string SecurityQuestion { get; set; }

        [Required(ErrorMessage = "Security answer is required.")]
        [StringLength(100, ErrorMessage = "Security answer cannot exceed 100 characters.")]
        public string SecurityAnswer { get; set; }

        public List<Transactions> Transactions { get; set; }


        // Constructor to initialize the account with a random account number
        public Accounts()
        {
            AccountNumber = GenerateRandomAccountNumber();
        }

        // Method to generate a random 11-digit account number
        private string GenerateRandomAccountNumber()
        {
            Random random = new Random();
            string accountNumber = "";

            for (int i = 0; i < 11; i++)
            {
                accountNumber += random.Next(0, 10).ToString(); // Generates a digit from 0 to 9
            }

            return accountNumber;
        }
    }
}
