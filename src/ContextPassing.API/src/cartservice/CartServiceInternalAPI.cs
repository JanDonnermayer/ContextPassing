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
            Connection = "STORAGE_CONNECTION"
        )]
        public static async Task<StringContentEntity> SetContext(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "post",
                Route = "checkout-context/{token}"
            )] HttpRequest req,
            string token
        )
        {
            var context = await req.Body
                .ReadAllTextAsync()
                .ConfigureAwait(false);

            return new StringContentEntity()
            {
                Content = context,
                PartitionKey = token,
                RowKey = token
            };
        }

        [FunctionName("checkout-context-get")]
        public static IActionResult GetContext(
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
            )] StringContentEntity entity
        )
        {
            return new ContentResult()
            {
                Content = entity.Content,
                ContentType = "application/json",
                StatusCode = 200
            };
        }
    }
}
