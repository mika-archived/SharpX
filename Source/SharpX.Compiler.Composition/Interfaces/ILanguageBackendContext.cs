using System;

using Microsoft.CodeAnalysis.CSharp;

using SharpX.Compiler.Composition.Enums;

namespace SharpX.Compiler.Composition.Interfaces
{
    public interface ILanguageBackendContext
    {
        /// <summary>
        ///     Registers the extension of the generated source.
        /// </summary>
        /// <param name="extension"></param>
        void RegisterExtension(string extension);

        /// <summary>
        ///     Registers the extension of the generated source based on the base class <paramref name="t" />.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="extension"></param>
        void RegisterExtensionFor(Type t, string extension);

        /// <summary>
        ///     Registers the generator of instantiating a new Source Context.
        /// </summary>
        /// <param name="generator"></param>
        void RegisterSourceContextGenerator(Func<ISourceContextGeneratorArgs, ISourceContext> generator);

        /// <summary>
        ///     Registers the generator of instantiating a new Source Context based on the base class <paramref name="t" />.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="generator"></param>
        void RegisterSourceContextGeneratorFor(Type t, Func<ISourceContextGeneratorArgs, ISourceContext> generator);

        /// <summary>
        ///     SharpX has completed the parsing and semantic analysis C# source, it will plug-in the new process based on the
        ///     syntax rules.
        ///     This method is called before the child nodes are visited.
        /// </summary>
        /// <param name="syntax"></param>
        /// <param name="action"></param>
        /// <param name="predicate"></param>
        void RegisterPreSyntaxAction(WellKnownSyntax syntax, Action<ILanguageSyntaxActionContext> action, Func<ILanguageSyntaxActionContext, bool>? predicate = null);

        /// <summary>
        ///     SharpX has completed the parsing and semantic analysis C# source, it will plug-in the new process based on the
        ///     syntax rules.
        ///     This method is called after the child nodes are visited.
        /// </summary>
        /// <param name="syntax"></param>
        /// <param name="action"></param>
        /// <param name="predicate"></param>
        void RegisterPostSyntaxAction(WellKnownSyntax syntax, Action<ILanguageSyntaxActionContext> action, Func<ILanguageSyntaxActionContext, bool>? predicate = null);

        /// <summary>
        ///     Register your own CSharpSyntaxWalker and use it.
        /// </summary>
        /// <param name="generator"></param>
        void RegisterCSharpSyntaxWalker(Func<ILanguageSyntaxWalkerContext, CSharpSyntaxWalker> generator);
    }
}