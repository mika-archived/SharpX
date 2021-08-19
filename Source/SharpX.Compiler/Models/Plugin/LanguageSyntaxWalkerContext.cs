using Microsoft.CodeAnalysis;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.Models.Plugin
{
    internal class LanguageSyntaxWalkerContext : ILanguageSyntaxWalkerContext
    {
        private readonly AssemblyContext _assembly;

        public LanguageSyntaxWalkerContext(SemanticModel model, ISourceContext context, AddOnlyCollection<IError> errors, AddOnlyCollection<IError> warnings, AssemblyContext assembly)
        {
            _assembly = assembly;
            SemanticModel = model;
            Errors = errors;
            Warnings = warnings;
            SourceContext = context;
        }

        public SemanticModel SemanticModel { get; }

        public ISourceContext SourceContext { get; private set; }

        public AddOnlyCollection<IError> Errors { get; }

        public AddOnlyCollection<IError> Warnings { get; }

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