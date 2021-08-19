using System;
using System.Collections.Generic;
using System.Text.Json;

using Microsoft.CodeAnalysis.CSharp;

using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.Models.Plugin
{
    internal class SourceRewriterContext : ISourceRewriterContext
    {
        private readonly List<Func<ILanguageSyntaxRewriterContext, CSharpSyntaxRewriter>> _generators;

        public SourceRewriterContext(JsonElement extra)
        {
            ExtraOptions = extra;
            _generators = new List<Func<ILanguageSyntaxRewriterContext, CSharpSyntaxRewriter>>();
        }

        public JsonElement ExtraOptions { get; }

        public void RegisterRewriter(Func<ILanguageSyntaxRewriterContext, CSharpSyntaxRewriter> generator)
        {
            _generators.Add(generator);
        }

        public IReadOnlyCollection<Func<ILanguageSyntaxRewriterContext, CSharpSyntaxRewriter>> GetGenerators()
        {
            return _generators.AsReadOnly();
        }
    }
}