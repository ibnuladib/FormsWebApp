namespace FormsWebApplication.Models
{
    public class CreateContactRequest
    {
        public string AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
