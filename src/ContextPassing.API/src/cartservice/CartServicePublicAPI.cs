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
using static ContextPassing.API.Configuration;

namespace ContextPassing
{
    public static class CartServicePublicAPI
    {
        private static readonly Lazy<HttpClient> client =
            new Lazy<HttpClient>(() => new HttpClient());


        [FunctionName("checkout-page")]
        public static async Task<IActionResult> FunnelPage(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                Route = "checkout-page/{token}"
            )] HttpRequest req,
            string token,
            [Table(
                "context",
                "{token}",
                "{token}",
                Connection = "STORAGE_CONNECTION"
            )] StringContentEntity sContext,
            ILogger log
        )
        {
            var context = JsonConvert
                .DeserializeObject<CheckoutContext>(sContext.Content);

            var funnelId = context.FunnelId;

            var funnelTemplate = await client.Value
                .GetStringAsync($"{BlobEndpoint}/funnels/{funnelId}")
                .ConfigureAwait(false);

            var content = context.Customer
                .GetType()
                .GetProperties()
                .ToDictionary(
                    prop => $"$({prop.Name})",
                    prop => prop.GetValue(context.Customer).ToString()
                )
                .Aggregate(
                    funnelTemplate,
                    (t, kvp) => t.Replace(kvp.Key, kvp.Value)
                );

            return new ContentResult()
            {
                Content = content,
                ContentType = "text/html"
            };
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
            var customer = await req.Body
                .ReadJsonAsync<Customer>()
                .ConfigureAwait(false);

            var context = new CheckoutContext(
                funnelId: funnelId,
                customer: customer
            );

            var token = Uri.EscapeDataString(
                context.GetSHA256Hash()
            );

            var contextContent = new StringContent(
                JsonConvert.SerializeObject(context)
            );

            await client.Value
                .PostAsync(
                    $"{CartServiceInternalApi}/checkout-context/{token}",
                    contextContent
                )
                .ConfigureAwait(false);

            var link = $"{CartServicePublicApi}/checkout-page/{token}";

            return new OkObjectResult(link);
        }
    }
}
