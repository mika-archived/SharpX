using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.Models.Plugin
{
    internal class LanguageSyntaxActionContext : ILanguageSyntaxActionContext
    {
        private readonly AssemblyContext _assembly;

        public bool ShouldStopPropagation { get; private set; }
        public bool ShouldStopPropagationIncludingSiblingActions { get; private set; }

        public LanguageSyntaxActionContext(CSharpSyntaxNode node, SemanticModel model, ISourceContext context, AddOnlyCollection<IError> errors, AddOnlyCollection<IError> warnings, AssemblyContext assembly)
        {
            Node = node;
            SemanticModel = model;
            Errors = errors;
            Warnings = warnings;
            ShouldStopPropagation = false;
            ShouldStopPropagationIncludingSiblingActions = false;
            SourceContext = context;
            _assembly = assembly;
        }

        public SemanticModel SemanticModel { get; }

        public CSharpSyntaxNode Node { get; }

        public ISourceContext SourceContext { get; private set; }

        public AddOnlyCollection<IError> Errors { get; }

        public AddOnlyCollection<IError> Warnings { get; }

        public void StopPropagation()
        {
            ShouldStopPropagation = true;
        }

        public void StopPropagationIncludingSiblingActions()
        {
            ShouldStopPropagationIncludingSiblingActions = true;
        }

        public void CreateOrGetContext(string name)
        {
            if (_assembly.HasContext<object>(name))
                SourceContext = _assembly.GetContext<object>(name);
            else
                SourceContext = _assembly.AddContext<object>(name);
        }

        public void CreateOrGetContext<T>(string name)
        {
            if (_assembly.HasContext<T>(name))
                SourceContext = _assembly.GetContext<T>(name);
            else
                SourceContext = _assembly.AddContext<T>(name);
        }

        public void CloseContext()
        {
            SourceContext = _assembly.Default;
        }

        public bool IsContextOpened()
        {
            return SourceContext != _assembly.Default;
        }
    }
}