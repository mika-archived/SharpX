using Microsoft.CodeAnalysis;

using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.Models.Plugin
{
    internal class LanguageSyntaxRewriterContext : ILanguageSyntaxRewriterContext
    {
        public LanguageSyntaxRewriterContext(Solution solution, SemanticModel model)
        {
            Solution = solution;
            SemanticModel = model;
        }

        public Solution Solution { get; }

        public SemanticModel SemanticModel { get; }
    }
}