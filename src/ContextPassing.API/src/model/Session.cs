namespace ContextPassing
{
    public class Session
    {
        public Session(string funnelId, Customer customer)
        {
            if (string.IsNullOrEmpty(funnelId))
                throw new System.ArgumentException($"'{nameof(funnelId)}' cannot be null or empty", nameof(funnelId));

            FunnelId = funnelId;
            Customer = customer ?? throw new System.ArgumentNullException(nameof(customer));
        }

        public string FunnelId { get; }

        public Customer Customer { get; }
    }
}
