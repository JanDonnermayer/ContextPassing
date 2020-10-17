namespace ContextPassing
{
    public class Customer
    {
        public Customer(
            string id,
            string email,
            string firstName,
            string lastName,
            string phoneNumber
        )
        {
            Id = id;
            this.Email = email;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.PhoneNumber = phoneNumber;
        }

        public string Id { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string PhoneNumber { get; }
    }
}
