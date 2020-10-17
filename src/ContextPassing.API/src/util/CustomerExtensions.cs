using Newtonsoft.Json;
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
           return Encoding.UTF8.GetString(sha.ComputeHash(bytes));
        }
    }
}
