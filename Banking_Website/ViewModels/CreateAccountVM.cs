namespace Banking_Website.ViewModels
{
    public class CreateAccountViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public decimal? InitialBalance { get; set; }
        public string SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }
    }

}
