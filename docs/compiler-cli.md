# Compiler CLI

SharpX provides a CLI command to run SharpX from the command line.
The command can be executed in the following format:

```bash
# Windows
$ sxc.exe

# macOS or Linux
$ dotnet exec sxc.dll
```

## Commands

### `sxc init`

Initializes the SharpX project.

```bash
$ sxc.exe init [path]
```

### `sxc build`

Build and Compile C# to any backend languages.

```bash
# build by project (SharpX Project Configuration)
$ sxc.exe build --project ./path/to/sxc.config.json

# build by solution (SharpX Solution Configuration)
$ sxc.exe build --solution ./path/to/sxc.sol.json
```

### `sxc watch`

Watch and Compile C# to any backend languages.
It detects file changes and automatically compiles them.

```bash
# build by project (SharpX Project Configuration)
$ sxc.exe watch --project ./path/to/sxc.config.json

# build by solution (SharpX Solution Configuration)
$ sxc.exe watch --solution ./path/to/sxc.sol.json
```
