using System.Collections.Immutable;

namespace SharpX.Compiler.Composition.Interfaces
{
    public interface ISourceRewriter
    {
        string Identifier { get; }

        ImmutableArray<string> SupportedIdentifiers { get; }

        void Initialize(ISourceRewriterContext context);
    }
}