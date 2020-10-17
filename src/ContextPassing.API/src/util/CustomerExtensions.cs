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
           var bytes = Encoding.Unicode.GetBytes(json);
           return Encoding.Unicode.GetString(sha.ComputeHash(bytes));
        }
    }
}
