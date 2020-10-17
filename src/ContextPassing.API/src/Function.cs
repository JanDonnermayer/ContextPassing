using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Linq;

namespace api
{
    public class FunnelLaunchContext
    {
        public FunnelLaunchContext(string nonce, Customer customer, string funnelId)
        {
            Nonce = nonce;
            Customer = customer;
            FunnelId = funnelId;
        }

        public string Nonce { get; }

        public string FunnelId { get; }

        public Customer Customer { get; }
    }

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

    public static class Function
    {
        private static readonly Lazy<HttpClient> client =
            new Lazy<HttpClient>(() => new HttpClient());

        private static string BlobEndpoint =>
            Environment.GetEnvironmentVariable("BLOB_STORAGE_ENDPOINT");

        private static string LocalEndpoint =>
            Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");


        [FunctionName("funnel-page")]
        public static async Task<IActionResult> FunnelPage(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "funnel-page/{nonce}")] HttpRequest req,
            string nonce,
            [Table("context", "{nonce}", "{nonce}", Connection = "STORAGE_CONNECTION")] FunnelLaunchContext context
            ILogger log
        )
        {
            var funnelId = context.FunnelId;

            var funnelTemplate = await client.Value
                .GetStringAsync($"{BlobEndpoint}/funnels/{funnelId}")
                .ConfigureAwait(false);

            var content = context.Customer
                .GetType()
                .GetProperties()
                .ToDictionary(
                    prop => prop.Name,
                    prop => prop.GetValue(context.Customer).ToString()
                )
                .Aggregate(
                    funnelTemplate,
                    (t, kvp) => t.Replace(kvp.Key, kvp.Value)
                );

            return new OkObjectResult(content);
        }

        [FunctionName("vendor-page")]
        public static async Task<IActionResult> VendorPage(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log
        )
        {
            var funnelId = "default-funnel.html";

            var funnelLink = await client.Value
                .GetStringAsync($"{LocalEndpoint}/funnel-link/{funnelId}")
                .ConfigureAwait(false);

            return "<button></button>"
        }

        [FunctionName("funnel-context-set")]
        [return: Table("context", "{nonce}", "{nonce}" Connection = "STORAGE_CONNECTION")]
        public static async Task<FunnelLaunchContext> SetContext(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "post", Route = "funnel-context/{nonce}"
            )] HttpRequest req,
            string nonce,
            ILogger log
        )
        {
            return await req.Body
                .ReadJsonAsync<FunnelLaunchContext>()
                .ConfigureAwait(false);
        }

        [FunctionName("funnel-context-get")]
        public static async Task<IActionResult> GetContext(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get", Route = "funnel-context/{nonce}"
            )] HttpRequest req,
            [Table("context", "{nonce}", "{nonce}", Connection = "STORAGE_CONNECTION")] FunnelLaunchContext context
            ILogger log
        )
        {
            return new OkObjectResult(context);
        }

        [FunctionName("funnel-link-get")]
        public static async Task<IActionResult> GetLink(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get", Route = "funnel-link/{funnelId}"
            )] HttpRequest req,
            string funnelId,
            ILogger log
        )
        {
            var customer = new Customer(
                req.Query["id"],
                req.Query["email"],
                req.Query["firstName"],
                req.Query["lastName"],
                req.Query["phoneNumber"]
            );

            var nonce = Uri.EscapeDataString(Guid.NewGuid().ToString());

            var context = new FunnelLaunchContext(
                nonce,
                customer,
                funnelId
            );

            var content = new StringContent(
                JsonConvert.SerializeObject(context)
            );

            await client.Value
                .PostAsync($"{LocalEndpoint}/funnel-context/{funnelId}/{nonce}", content)
                .ConfigureAwait(false);

            var link = $"{LocalEndpoint}/funnelPage/{nonce}";

            return new OkObjectResult(link);
        }
    }

    public static class StreamExtensions
    {
        public static async Task<string> ReadAllTextAsync(this Stream stream)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            using var sr = new StreamReader(stream);
            return await sr.ReadToEndAsync().ConfigureAwait(false);
        }

        public static async Task<T> ReadJsonAsync<T>(this Stream stream)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            var json = await stream
                .ReadAllTextAsync()
                .ConfigureAwait(false);

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
