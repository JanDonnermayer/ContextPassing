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

    public static class MerchantPublicAPI
    {
        private static readonly Lazy<HttpClient> client =
            new Lazy<HttpClient>(() => new HttpClient());

        private static string CartServicePublicApi =>
            Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");

        [FunctionName("landing-page")]
        public static async Task<IActionResult> LandingPage(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log
        )
        {
            var funnelId = "default-funnel.html";

            var sampleCustomer = new Customer(
                id: Guid.NewGuid().ToString(),
                email: "some@mail.com",
                firstName: "1337Cart",
                lastName: "User",
                phoneNumber: "012-1234"
            );

            var response = await client.Value
                .PostAsJsonAsync($"{CartServicePublicApi}/checkout-link/{funnelId}", sampleCustomer)
                .ConfigureAwait(false);

            var checkOutLink = await response.Content.ReadAsStringAsync();

            var markUp =
                $@"<!DOCTYPE html>
                <header>
                </header>
                <body>
                    <button (click)='() => window.open({checkOutLink})'>Launch Checkout</button>
                </body>";

            return new ContentResult(){
                Content = markUp,
                ContentType = "application/html",
                StatusCode = 200
            };
        }
    }
}
