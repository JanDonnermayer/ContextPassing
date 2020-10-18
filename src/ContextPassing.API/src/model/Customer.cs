using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace ContextPassing
{
    public class Customer
    {
        public Customer(
            string id,
            string email,
            string firstName,
            string lastName
        )
        {
            this.Id = id;
            this.Email = email;
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        public string Id { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
    }
}
