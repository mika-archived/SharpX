using Microsoft.CodeAnalysis;

namespace SharpX.Compiler.Composition.Interfaces
{
    public interface ILanguageSyntaxRewriterContext
    {
        Solution Solution { get; }

        SemanticModel SemanticModel { get; }
    }
}