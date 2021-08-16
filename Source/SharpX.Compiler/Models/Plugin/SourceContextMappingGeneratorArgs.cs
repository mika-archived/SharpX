using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.Models.Plugin
{
    public record SourceContextMappingGeneratorArgs(string Variant, string OriginalName) : ISourceContextMappingArgs { }
}