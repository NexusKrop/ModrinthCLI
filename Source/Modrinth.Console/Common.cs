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
        public static readonly Modrinth.RestClient.IModrinthApi api = Modrinth.RestClient.ModrinthApi.NewClient(userAgent: "NexusKrop-ModrinthCLI");
        public const string Command = "modrinth";

        static Common()
        {
            // To Modrinth Staff: This is due to .NET Do not let me to write anything more!
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("NexusKrop-ModrinthCLI", Assembly.GetExecutingAssembly().GetName().Version?.ToString()));
        }

        public static void Print(string message)
        {
            System.Console.WriteLine("{0}: {1}", Command, message);
        }

        public static void PrintError(string message)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            Print(message);
            System.Console.ResetColor();
        }
    }
}