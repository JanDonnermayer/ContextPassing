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
