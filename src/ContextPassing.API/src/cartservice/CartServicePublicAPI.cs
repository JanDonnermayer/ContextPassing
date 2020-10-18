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


        [FunctionName("funnel-page")]
        public static async Task<IActionResult> FunnelPage(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                Route = "funnel-page/{token}"
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
            try
            {
                var context = JsonConvert
                    .DeserializeObject<Session>(sContext.Content);

                var content = $@"
                    <h1>Funnel Page</h1>
                    <table>
                    <tr>
                        <td><label>First Name</label></td>
                        <td><span>{context.Customer.FirstName}</span></td>
                    </tr>
                    <tr>
                        <td><label>Last Name</label></td>
                        <td><span>{context.Customer.LastName}</span></td>
                    </tr>
                    <tr>
                        <td><label>Email</label></td>
                        <td><span>{context.Customer.Email}</span></td>
                    </tr>
                    </table>";

                return new ContentResult()
                {
                    Content = content,
                    ContentType = "text/html"
                };
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return new ObjectResult(ex.Message);
            }

        }

        [FunctionName("funnel-link")]
        public static async Task<IActionResult> CheckoutLink(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "post",
                Route = "funnel-link/{funnelId}"
            )] HttpRequest req,
            string funnelId,
            ILogger log
        )
        {
            var customer = await req.Body
                .ReadJsonAsync<Customer>()
                .ConfigureAwait(false);

            var context = new Session(
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
                    $"{CartServiceInternalApi}/session/{token}",
                    contextContent
                )
                .ConfigureAwait(false);

            var link = $"{CartServicePublicApi}/funnel-page/{token}";

            return new OkObjectResult(link);
        }
    }
}
