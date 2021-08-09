# Compiler CLI

SharpX provides a CLI command to run SharpX from the command line.
The command can be executed in the following format:

```bash
# Windows
$ ssc.exe

# macOS or Linux
$ dotnet exec ssc.dll
```

## Commands

### `ssc init`

Initializes the SharpX project.

```bash
$ ssc.exe init
```

### `ssc build`

Build and Compile C# Shaders to ShaderLab HLSL Shaders.

```bash
# build by project (ssconfig)
$ ssc.exe build --project ./path/to/ssconfig.json

# build by sources
$ ssc.exe build --src **/*.cs --out dist --references /path/to/SharpX.Library.ShaderCommon.dll,/path/to/SharpX.Library.ShaderLab.dll --plugins /path/to/SharpX.Compiler.ShaderLab.dll --target ShaderLab
```

It is recommended to build in a project format using the following ssconfig (JSON format) file.

```json
{
  "Sources": ["**/*.cs"],
  "Out": "dist",
  "References": [
    "/path/to/SharpX.Library.ShaderCommon.dll",
    "/path/to/SharpX.Library.ShaderLab.dll"
  ],
  "Plugins": ["/path/to/SharpX.Compiler.ShaderLab.dll"],
  "Target": "ShaderLab"
}
```
