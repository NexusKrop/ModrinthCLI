using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using System.Text.Json;

namespace Modrinth.Console.CommandLine
{
    [Verb("get")]
    public class GetOptions
    {
        [Value(0, MetaName = "slug", Required = true, HelpText = "The slug of the project")]
        public string Slug { get; set; } = "";

        [Option('d', "show-dependencies", Required = false, HelpText = "Show dependencies")]
        public bool Dependencies { get; set; }

        public static async Task<int> Run(GetOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.Slug))
            {
                Common.PrintError("Invalid slug");
                return ReturnCodes.ArgumentError;
            }

            var uri = $"https://api.modrinth.com/v2/project/{options.Slug}";
            var project = await Common.client.GetAsync(uri);

            if (project.StatusCode == HttpStatusCode.NotFound)
            {
                Common.PrintError("No such project");
                return ReturnCodes.ArgumentError;
            }

            JsonDocument doc;
            using (var stream = await project.Content.ReadAsStreamAsync())
            {
                doc = await JsonDocument.ParseAsync(stream);
            }

            ShowProject(doc.RootElement);
            if (options.Dependencies)
            {
                await GetDependencies(uri);
            }

            return 0;
        }

        public static async Task GetDependencies(string uri)
        {
            var dependentUri = $"{uri}/dependencies";

            var deps = await Common.client.GetAsync(dependentUri);

            if (deps.StatusCode == HttpStatusCode.NotFound)
            {
                Common.PrintError("No dependency information available");
            }

            JsonDocument doc;
            using (var stream = await deps.Content.ReadAsStreamAsync())
            {
                doc = await JsonDocument.ParseAsync(stream);
            }

            System.Console.WriteLine("Dependencies:");
            System.Console.WriteLine("---------------------------------------------");

            foreach (var project in doc.RootElement.GetProperty("versions").EnumerateArray())
            {
                ShowProject(project);
                System.Console.WriteLine("---------------------------------------------");
            }
        }

        public static ConsoleColor GetSideColour(string side)
        {
            switch (side)
            {
                case "required":
                    return ConsoleColor.Cyan;
                case "optional":
                    return ConsoleColor.Green;
                case "unsupported":
                    return ConsoleColor.Red;
                default:
                    return ConsoleColor.DarkGray;
            }
        }

        public static void ShowProject(JsonElement re)
        {
            System.Console.Write(re.GetProperty("slug").GetString());
            System.Console.Write(" \"{0}\"", re.GetProperty("title").GetString());
            System.Console.ForegroundColor = ConsoleColor.Blue;
            System.Console.Write(" (ID {0}) ", re.GetProperty("id").GetString());
            System.Console.ForegroundColor = ConsoleColor.Magenta;
            System.Console.WriteLine(re.GetProperty("status").GetString());

            System.Console.ForegroundColor = ConsoleColor.Gray;
            var categories = re.GetProperty("categories");
            foreach (var category in categories.EnumerateArray())
            {
                System.Console.Write("#{0} ", category.GetString());
            }
            System.Console.WriteLine();
            System.Console.WriteLine();

            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("Published {0}", DateTime.Parse(re.GetProperty("published").GetString()));
            System.Console.WriteLine("Updated {0}", DateTime.Parse(re.GetProperty("updated").GetString()));
            System.Console.ResetColor();

            System.Console.Write("   Client: ");
            var client = re.GetProperty("client_side").GetString();
            System.Console.ForegroundColor = GetSideColour(client);
            System.Console.WriteLine(client);
            System.Console.ResetColor();

            var server = re.GetProperty("server_side").GetString();
            System.Console.Write("   Server: ");
            System.Console.ForegroundColor = GetSideColour(server);
            System.Console.WriteLine(server);
            System.Console.ResetColor();

            System.Console.WriteLine(re.GetProperty("description").GetString());
        }
    }
}