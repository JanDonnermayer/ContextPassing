namespace ContextPassing
{
    public class CheckoutContext
    {
        public CheckoutContext(string funnelId, Customer customer)
        {
            FunnelId = funnelId;
            Customer = customer;
        }

        public string FunnelId { get; }

        public Customer Customer { get; }
    }
}
