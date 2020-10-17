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

namespace ContextPassing
{
    public static class CartServicePublicAPI
    {
        private static readonly Lazy<HttpClient> client =
            new Lazy<HttpClient>(() => new HttpClient());

        private static string BlobEndpoint =>
            Environment.GetEnvironmentVariable("BLOB_STORAGE_ENDPOINT");

        private static string CartServicePublicApi =>
            Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");

        private static string CartServiceInternalApi =>
            Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");


        [FunctionName("checkout-page")]
        public static async Task<IActionResult> FunnelPage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "checkout-page/{nonce}")] HttpRequest req,
            string nonce,
            [Table("context", "{nonce}", "{nonce}", Connection = "STORAGE_CONNECTION")] CheckoutContext context,
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

        [FunctionName("checkout-link")]
        public static async Task<IActionResult> CheckoutLink(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "post",
                Route = "checkout-link/{funnelId}"
            )] HttpRequest req,
            string funnelId,
            ILogger log
        )
        {
            var nonce = Uri.EscapeDataString(Guid.NewGuid().ToString());

            var customer = await req.Body
                .ReadJsonAsync<Customer>()
                .ConfigureAwait(false);

            var context = new CheckoutContext(
                nonce,
                funnelId,
                customer
            );

            var contextJson = new StringContent(
                JsonConvert.SerializeObject(context)
            );

            await client.Value
                .PostAsync($"{CartServiceInternalApi}/checkout-context/{funnelId}/{nonce}", contextJson)
                .ConfigureAwait(false);

            var link = $"{CartServicePublicApi}/checkout-page/{nonce}";

            return new OkObjectResult(link);
        }
    }
}
