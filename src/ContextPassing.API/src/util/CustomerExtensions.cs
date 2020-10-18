using Newtonsoft.Json;
using System;
using System.Text;

namespace ContextPassing
{
    public static class ObjectExtensions
    {
        public static string GetSHA256Hash<T>(this T obj)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var json = JsonConvert.SerializeObject(obj);
            var bytes = Encoding.UTF8.GetBytes(json);
            var hash = sha.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
