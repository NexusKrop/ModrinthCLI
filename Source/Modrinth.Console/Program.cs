using Modrinth.Console.CommandLine;

using CommandLine;

return await Parser.Default.ParseArguments<PingOptions, SearchOptions, GetOptions, PeekOptions, Download>(args)
    .MapResult<PingOptions, SearchOptions, GetOptions, PeekOptions, Download, Task<int>>(
    async (PingOptions x) => await PingOptions.Run(x),
    async (SearchOptions x) => await SearchOptions.Run(x),
    async (GetOptions x) => await GetOptions.Run(x),
    async (PeekOptions x) => await x.Run(),
    async (Download x) => await x.Execute(),
    errs => Task.FromResult(1)
);