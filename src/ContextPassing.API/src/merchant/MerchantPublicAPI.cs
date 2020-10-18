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

    public static class MerchantPublicAPI
    {
        private static readonly Lazy<HttpClient> client =
            new Lazy<HttpClient>(() => new HttpClient());


        [FunctionName("landing-page")]
        public static async Task<IActionResult> LandingPage(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log
        )
        {
            log.LogInformation("API: " + CartServicePublicApi);

            var funnelId = "default-funnel.html";

            var sampleCustomer = new Customer(
                id: Guid.NewGuid().ToString(),
                email: "some@mail.com",
                firstName: "1337Cart",
                lastName: "User"
            );

            var response = await client.Value
                .PostAsJsonAsync($"{CartServicePublicApi}/funnel-link/{funnelId}", sampleCustomer)
                .ConfigureAwait(false);

            var checkOutLink = await response.Content
                .ReadAsStringAsync()
                .ConfigureAwait(false);

            var markUp =
                $@"<!DOCTYPE html>
                <body>
                    <a href=""{checkOutLink}"">
                        <button>Launch Checkout</button>
                    </a>
                </body>";

            return new ContentResult()
            {
                Content = markUp,
                ContentType = "text/html"
            };
        }
    }
}
