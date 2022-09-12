using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CommandLine;

namespace Modrinth.Console.CommandLine
{
    [Verb("search", HelpText = "Search and filter a project based on the options given.")]
    public class SearchOptions
    {
        public static readonly string[] AcceptedSorts = {
            "relevance",
            "downloads",
            "follows",
            "newest",
            "updated"
        };

        [Value(0, MetaName = "query", HelpText = "The string to search.", Required = true)]
        public string Query { get; set; } = "";

        [Option('s', "sort", HelpText = "Sort by. Accepts relevance, downloads, follows, newest, updated")]
        public string Sort { get; set; } = "relevance";

        [Option('p', "page", HelpText = "The page to show", Default = 1)]
        public int Page { get; set; } = 1;

        [Option('P', "page-size", HelpText = "The size of the page.", Default = 10)]
        public int PageSize { get; set; } = 10;

        [Option('t', "type", HelpText = "Project type", Required = false)]
        public string? ProjectType { get; set; }

        [Option('v', "version", HelpText = "The minecraft version too look for", Required = false)]
        public string? MinecraftVersion { get; set; }

        public static async Task<int> Run(SearchOptions opts)
        {
            if (!AcceptedSorts.Contains(opts.Sort))
            {
                System.Console.WriteLine("Sort not allowed. Use --help to get help.");
                return 2;
            }

            var indexFrom = opts.PageSize * (opts.Page - 1);

            var facet = new StringBuilder();

            if (opts.ProjectType != null)
            {
                facet.AppendFormat("[\"project_type:{0}\"]", opts.ProjectType);
                facet.Append(',');
            }

            if (opts.MinecraftVersion != null)
            {
                facet.AppendFormat("[\"versions:{0}\"]", opts.MinecraftVersion);
                facet.Append(',');
            }

            var ftStr = facet.ToString();

            if (!string.IsNullOrWhiteSpace(ftStr))
            {
                ftStr = ftStr.Remove(ftStr.LastIndexOfAny(new char[] { ',' }), 1);
            }

            var sb = new StringBuilder();
            sb.Append("https://api.modrinth.com/v2/search");
            sb.AppendFormat("?index={0}", opts.Sort);
            sb.AppendFormat("&limit={0}", opts.PageSize);
            sb.AppendFormat("&offset={0}", indexFrom);
            sb.AppendFormat("&query={0}", opts.Query);
            if (!string.IsNullOrWhiteSpace(ftStr)) sb.AppendFormat("&facets=[{0}]", ftStr);

            var url = sb.ToString();

            System.Console.WriteLine(url);
            var result = await Common.client.GetStreamAsync(url);

            JsonDocument document;

            using (result)
            {
                try
                {
                    document = await JsonDocument.ParseAsync(result);
                }
                catch (HttpRequestException hrex)
                {
                    System.Console.WriteLine("Failure! {0}", hrex.Message);
                    return 3;
                }
            }

            var hits = document.RootElement.GetProperty("hits");
            foreach (var hit in hits.EnumerateArray())
            {
                System.Console.ForegroundColor = ConsoleColor.Magenta;
                System.Console.Write("{0} ", hit.GetProperty("slug").GetString());
                System.Console.ForegroundColor = ConsoleColor.Cyan;
                System.Console.Write("[{0}] ", hit.GetProperty("project_type").GetString());
                System.Console.ForegroundColor = ConsoleColor.White;
                System.Console.WriteLine("{0}", hit.GetProperty("latest_version").GetString());
                System.Console.ForegroundColor = ConsoleColor.Gray;
                System.Console.WriteLine("  {0}: {1}", hit.GetProperty("title").GetString(), hit.GetProperty("description").GetString());
                System.Console.WriteLine();
            }

            return 0;
        }
    }
}