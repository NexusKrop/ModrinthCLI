# Modrinth CLI

![GitHub Workflow Status](https://img.shields.io/github/workflow/status/NexusKrop/ModrinthCLI/.NET?logo=github&style=flat-square)
[![GitHub](https://img.shields.io/github/license/NexusKrop/ModrinthCLI?style=flat-square)](LICENSE)
![GitHub issues](https://img.shields.io/github/issues/NexusKrop/ModrinthCLI?style=flat-square)
![GitHub commit activity](https://img.shields.io/github/commit-activity/m/NexusKrop/ModrinthCLI?style=flat-square)
![GitHub tag (latest by date)](https://img.shields.io/github/v/tag/NexusKrop/ModrinthCLI?style=flat-square)

This is a halfway done (basically usable but function limited) implementation of a command-line client for interacting with Modrinth CLI in .NET (C#).

## Downloads

Non-production ready builds are available at the [Releases](https://github.com/NexusKrop/ModrinthCLI/releases) page. Windows and GNU/Linux builds are available.

You can also just download the release tarball or clone the source code and try `dotnet run`.

## Features

- [x] Search (`search`)
- [x] Pinging (`ping`)
- [x] Get project details (`get`)
- [x] Download project file (`download`)

## Usage

```sh
modrinth [options]

# See help using:
modrinth help
```

## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License

This project is licensed under [Apache-2.0](LICENSE) license.

## Disclaimer

This product is not an official Mojang product nor an official Modrinth product. Use at your own risk.