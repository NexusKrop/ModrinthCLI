using System.Security.AccessControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;

namespace Modrinth.Console.CommandLine
{
    [Verb("download", HelpText = "Download a file from a project's latest version.")]
    public class Download
    {
        [Value(0, MetaName = "slug", HelpText = "Project slug", Required = true)]
        public string Slug { get; set; }

        [Option('l', "loader", HelpText = "The mod loader to search for.", Required = false)]
        public string? Loader { get; set; }

        [Option('v', "mcversion", HelpText = "The Minecraft version to search for.", Required = false)]
        public string? McVersion { get; set; }

        [Option('t', "version-type", HelpText = "The type of the version to search for", Required = false, Default = "release")]
        public string VersionType { get; set; } = "release";

        public async Task<int> Execute()
        {
            RestClient.Models.Version[] versions;
            try
            {
                versions = await Common.api.GetProjectVersionListAsync(Slug);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("404"))
                {
                    Common.PrintError("No such project");
                    return ReturnCodes.ArgumentError;
                }

                Common.PrintError(ex.ToString());
                return ReturnCodes.GenericError;
            }

            var vtSuccess = Enum.TryParse<RestClient.Models.Enums.VersionType>(VersionType, true, out var vt);
            if (!vtSuccess)
            {
                Common.PrintError("Invalid version type; should be \"alpha\", \"beta\" or \"release\" (default)");
                return ReturnCodes.ArgumentError;
            }

            if (versions.Length == 0)
            {
                Common.PrintError("No versions available for this project");
                return ReturnCodes.ArgumentError;
            }

            var qualified = versions.Where(version =>
            {
                var qualified = true;

                if (version.VersionType != vt)
                {
                    qualified = false;
                }

                if (McVersion != null && !version.GameVersions.Contains(McVersion))
                {
                    qualified = false;
                }

                if (Loader != null && !version.Loaders.Contains(Loader))
                {
                    qualified = false;
                }

                return qualified;
            });
            if (qualified.Count() == 0)
            {
                Common.PrintError("No versions match your filter");
                return ReturnCodes.ArgumentError;
            }

            var primaries = qualified.First().Files.Where(x => x.Primary);
            if (primaries.Count() == 0)
            {
                Common.PrintError("No primary files for the first qualified version!");
                return ReturnCodes.ArgumentError;
            }

            var file = primaries.First();
            using (var stream = await Common.client.GetStreamAsync(file.Url))
            {
                using (var fl = File.Create(file.FileName))
                {
                    await stream.CopyToAsync(fl);
                }
            }

            return 0;
        }
    }
}