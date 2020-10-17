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

    public static class CartServiceInternalAPI
    {
        [FunctionName("checkout-context-set")]
        [return: Table(
            tableName: "context",
            partitionKey: "{token}",
            rowKey: "{token}",
            Connection = "STORAGE_CONNECTION"
        )]
        public static async Task<CheckoutContext> SetContext(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "post",
                Route = "checkout-context/{token}"
            )] HttpRequest req
        )
        {
            return await req.Body
                .ReadJsonAsync<CheckoutContext>()
                .ConfigureAwait(false);
        }

        [FunctionName("checkout-context-get")]
        public static async Task<IActionResult> GetContext(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                Route = "checkout-context/{token}"
            )] HttpRequest req,
            [Table(
                tableName: "context",
                partitionKey: "{token}",
                rowKey: "{token}",
                Connection = "STORAGE_CONNECTION"
            )] CheckoutContext context
        )
        {
            return new OkObjectResult(context);
        }
    }
}
