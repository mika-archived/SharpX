using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using SharpX.Compiler.Composition.Abstractions;

namespace SharpX.Compiler.Composition.Interfaces
{
    public interface ILanguageSyntaxActionContext
    {
        SemanticModel SemanticModel { get; }

        CSharpSyntaxNode Node { get; }

        ISourceContext SourceContext { get; }

        Dictionary<string, object> ContextStore { get; }

        AddOnlyCollection<IError> Errors { get; }

        AddOnlyCollection<IError> Warnings { get; }

        void StopPropagation();

        void StopPropagationIncludingSiblingActions();

        void SwitchDefaultVisit(bool visit);

        void Visit(CSharpSyntaxNode node);

        void CreateOrGetContext(string name);

        void CreateOrGetContext<T>(string name);

        void CloseContext();

        bool IsContextOpened();
    }
}