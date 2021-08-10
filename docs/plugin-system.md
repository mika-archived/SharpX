# Plugin System

SharpX allows you to add new features through your plugin system.
For example, the compilation part from C # to ShaderLab is also implemented through the plugin system.

## Add New Compile Destination

For example, if you want to output from C # to another language such as HSLS or GLSL, you can add the corresponding processing by implementing the `ILanguageBackendPlugin` interface.
The `ILanguageBackendPlugin` interface includes access to C# compiler data, output source context, structure, and more.

## Add New Source Transformer
