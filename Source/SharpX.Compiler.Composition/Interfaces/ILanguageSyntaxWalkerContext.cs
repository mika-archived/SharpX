using Microsoft.CodeAnalysis;

using SharpX.Compiler.Composition.Abstractions;

namespace SharpX.Compiler.Composition.Interfaces
{
    public interface ILanguageSyntaxWalkerContext
    {
        Compilation Compilation { get; }

        SemanticModel SemanticModel { get; }

        ISourceContext SourceContext { get; }

        AddOnlyCollection<IError> Errors { get; }

        AddOnlyCollection<IError> Warnings { get; }

        void CreateOrGetContext(string name);

        void CreateOrGetContext<T>(string name);

        void CloseContext();

        bool IsContextOpened();
    }
}