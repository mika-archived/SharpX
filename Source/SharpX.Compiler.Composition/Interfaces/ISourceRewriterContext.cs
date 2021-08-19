using System;
using System.Text.Json;

using Microsoft.CodeAnalysis.CSharp;

namespace SharpX.Compiler.Composition.Interfaces
{
    public interface ISourceRewriterContext
    {
        /// <summary>
        ///     Get extra compiler options
        /// </summary>
        JsonElement ExtraOptions { get; }

        /// <summary>
        ///     Register your own CSharpSyntaxRewriter and use it.
        /// </summary>
        /// <param name="generator"></param>
        void RegisterRewriter(Func<ILanguageSyntaxRewriterContext, CSharpSyntaxRewriter> generator);
    }
}