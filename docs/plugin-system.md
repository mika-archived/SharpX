# Plugin System

SharpX allows you to add new features through your plugin system.
For example, the compilation part from C # to ShaderLab is also implemented through the plugin system.

## Language AST Features

### Transformer

The Transformer rewrites the C# source code entered into SharpX. Compiler just before compilation, thereby eliminating unnecessary variables and methods from the compilation process.
This is done through Roslyn's `CSharpSyntaxRewriter`, which registers the process in a class file with the `ISourceRewriter` interface and the `SourceRewriter` attribute.

### Language Backend

The Language Backend is the part that serves as the transpiler and compiler that does the work of converting the C# syntax tree to the syntax tree of another language.
This is usually done by a class that implements `SharpXSyntaxWalker`, and is registered through the `ILanguageBackend` interface and the `LanguageBackend` attribute.

