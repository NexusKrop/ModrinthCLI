using Modrinth.Console.CommandLine;

using CommandLine;

var result = await Task.Run<ParserResult<object>>(() => Parser.Default.ParseArguments<PingOptions, SearchOptions, GetOptions>(args));
return await result.MapResult<PingOptions, SearchOptions, GetOptions, Task<int>>(
    async (PingOptions x) => await PingOptions.Run(x),
    async (SearchOptions x) => await SearchOptions.Run(x),
    async (GetOptions x) => await GetOptions.Run(x),
    errs => Task.FromResult(1)
);

