namespace ContextPassing
{
    public class CheckoutContext
    {
        public CheckoutContext(string nonce, string funnelId, Customer customer)
        {
            Nonce = nonce;
            FunnelId = funnelId;
            Customer = customer;
        }

        public string Nonce { get; }

        public string FunnelId { get; }

        public Customer Customer { get; }
    }
}
