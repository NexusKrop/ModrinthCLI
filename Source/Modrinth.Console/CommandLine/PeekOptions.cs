using System.Net;
using System.Security.AccessControl;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using System.Text.Json;

namespace Modrinth.Console.CommandLine
{
    [Verb("peek")]
    public class PeekOptions
    {
        [Value(0, MetaName = "project", HelpText = "The slug of the project to peek.", Required = true)]
        public string Project { get; set; } = "";

        [Option('v', "mcversion", HelpText = "The minecraft version to peek.", Required = false)]
        public string? MCVersion { get; set; }

        [Option('l', "loader", Required = false, HelpText = "The mod loader to peek.")]
        public string? ModLoader { get; set; }

        [Option('f', "featured", Required = false, HelpText = "Featured filter. Specify to limit to only featured versions. Not compatible with --unfeatured")]
        public bool Featured { get; set; }

        [Option('u', "un-featured", Required = false, HelpText = "Un-featured filter. Specify to limit to only un-featured versions.")]
        public bool UnFeatured { get; set; }

        public string AssembleUri()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("https://api.modrinth.com/v2/project/{0}/version?", Project);

            if (MCVersion != null)
            {
                sb.AppendFormat("game_versions=[\"{0}\"]", MCVersion);
                sb.Append('&');
            }

            if (ModLoader != null)
            {
                sb.AppendFormat("loaders[\"{0}\"]", ModLoader);
                sb.Append('&');
            }

            if (Featured)
            {
                sb.Append("featured=true&");
            }

            if (UnFeatured)
            {
                sb.Append("featured=false");
            }

            var uri = sb.ToString();
            if (uri.EndsWith("&"))
            {
                uri.Remove(uri.LastIndexOfAny(new char[] { '&' }));
            }

            return uri;
        }

        public async Task<int> Run()
        {
            if (Featured && UnFeatured)
            {
                Common.PrintError("Both \"featured\" and \"un-featured\" specified");
                return ReturnCodes.ArgumentError;
            }

            var response = await Common.client.GetAsync(AssembleUri());
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                Common.PrintError("No such project");
                return ReturnCodes.ArgumentError;
            }

            JsonDocument doc;
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                doc = await JsonDocument.ParseAsync(stream);
            }

            var re = doc.RootElement;

            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("===——————————————————————————===");
            System.Console.ResetColor();

            foreach (var version in re.EnumerateArray())
            {
                ShowVersion(version);
            }

            return 0;
        }

        public static ConsoleColor GetVersionTypeColour(string type)
        {
            switch (type)
            {
                case "release":
                    return ConsoleColor.Green;
                case "beta":
                    return ConsoleColor.Yellow;
                case "alpha":
                    return ConsoleColor.Red;
                default:
                    return ConsoleColor.Gray;
            }
        }

        public static void ShowVersion(JsonElement version)
        {

            System.Console.ForegroundColor = ConsoleColor.Cyan;
            System.Console.Write("\"{0}\" (#{1})", version.GetProperty("name").GetString(), version.GetProperty("id").GetString());
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.Write(" {0} ", version.GetProperty("version_number").GetString());

            var type = version.GetProperty("version_type").GetString();
            System.Console.ForegroundColor = GetVersionTypeColour(type);
            System.Console.WriteLine(type);
            System.Console.ResetColor();

            System.Console.ForegroundColor = ConsoleColor.Gray;
            System.Console.WriteLine("  Published {0}", DateTime.Parse(version.GetProperty("date_published").GetString()));
            System.Console.WriteLine("  Downloaded {0} times", version.GetProperty("downloads").GetInt32());
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine();
            System.Console.WriteLine("Files: ");

            foreach (var file in version.GetProperty("files").EnumerateArray())
            {
                ShowFile(file);
            }

            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("===——————————————————————————===");
            System.Console.ResetColor();
        }

        public static void ShowFile(JsonElement file)
        {
            System.Console.ForegroundColor = ConsoleColor.Magenta;
            System.Console.Write("  {0} ", file.GetProperty("filename").GetString());

            if (file.GetProperty("primary").GetBoolean())
            {
                System.Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.Write(" [primary] ");
            }

            System.Console.ForegroundColor = ConsoleColor.Cyan;
            System.Console.WriteLine(" {0}B", file.GetProperty("size").GetInt64());
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("  {0}    {1}", file.GetProperty("url").GetString(), file.GetProperty("hashes").GetProperty("sha512").GetString());
            System.Console.WriteLine();
        }
    }
}