using System.IO;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace ContextPassing
{
    public class StringContentEntity : TableEntity
    {
        public string Content {get; set;}
    }

}
