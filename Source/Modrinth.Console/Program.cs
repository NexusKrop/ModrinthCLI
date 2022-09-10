using Modrinth.Console.CommandLine;

using CommandLine;

var result = await Task.Run<ParserResult<object>>(() => Parser.Default.ParseArguments<PingOptions, SearchOptions, GetOptions, PeekOptions>(args));
return await result.MapResult<PingOptions, SearchOptions, GetOptions, PeekOptions, Task<int>>(
    async (PingOptions x) => await PingOptions.Run(x),
    async (SearchOptions x) => await SearchOptions.Run(x),
    async (GetOptions x) => await GetOptions.Run(x),
    async (PeekOptions x) => await x.Run(),
    errs => Task.FromResult(1)
);

