using System.Reflection;
using System.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Modrinth.Console
{
    public static class Common
    {
        public static readonly HttpClient client = new();

        static Common()
        {
            // To Modrinth Staff: This is due to .NET Do not let me to write anything more!
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("NexusKrop-ModrinthCLI", Assembly.GetExecutingAssembly().GetName().Version?.ToString()));
        }
    }
}