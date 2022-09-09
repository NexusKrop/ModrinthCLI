using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using CommandLine;

namespace Modrinth.Console.CommandLine
{
    [Verb("ping")]
    public class PingOptions
    {
        public static async Task<int> Run(PingOptions options)
        {
            var result = await Common.client.GetStreamAsync("https://api.modrinth.com/");

            if (result == null)
            {
                System.Console.WriteLine("Failed to ping server");
                return 1;
            }

            JsonDocument document;

            using (result)
            {
                document = await JsonDocument.ParseAsync(result);
            }

            System.Console.ForegroundColor = ConsoleColor.Cyan;
            System.Console.WriteLine(document.RootElement.GetProperty("about").GetString());
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("Build: {0} (v{1})", document.RootElement.GetProperty("name").GetString(),
                document.RootElement.GetProperty("version").GetString());
            System.Console.WriteLine("Documentation: {0}", document.RootElement.GetProperty("documentation").GetString());
            System.Console.ResetColor();

            return 0;
        }
    }
}