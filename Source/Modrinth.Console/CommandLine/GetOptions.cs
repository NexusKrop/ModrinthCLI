using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;

namespace Modrinth.Console.CommandLine
{
    [Verb("get")]
    public class GetOptions
    {
        [Value(0, MetaName = "slug", Required = true, HelpText = "The slug of the project")]
        public string Slug { get; set; } = "";

        public static async Task<int> Run(GetOptions options)
        {
            // TODO

            return 0;
        }
    }
}