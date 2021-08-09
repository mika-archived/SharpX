using System;
using System.Collections.Generic;
using System.Linq;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Enums;
using SharpX.Compiler.Composition.Interfaces;
using SharpX.Compiler.Models.Plugin;

namespace SharpX.Compiler.Models
{
    internal class AssemblyContext
    {
        private readonly Dictionary<string, ISourceContext> _contexts;
        private readonly SharpXPluginHost _host;

        public ISourceContext Default => _contexts["source"];

        private ILanguageBackend CurrentLanguageBackend => _host.CurrentLanguageBackend!;

        private LanguageBackendContext CurrentLanguageBackendContext => (LanguageBackendContext) _host.CurrentLanguageBackendContext!;

        public AssemblyContext(SharpXPluginHost host)
        {
            _host = host;
            _contexts = new Dictionary<string, ISourceContext> { { "source", new DefaultSourceContext() } };
        }

        public ISourceContext AddContext<T>(string name)
        {
            if (HasContext<T>(name, out var fullName))
                throw new ArgumentException($"{fullName} is already exists in context");

            _contexts.Add(fullName, CurrentLanguageBackendContext.GetSourceGeneratorFor<T>().Invoke(new SourceContextGeneratorArgs(name, fullName)));
            return _contexts[fullName];
        }

        private bool HasContext<T>(string name, out string fullName)
        {
            fullName = name.Replace("{extension}", CurrentLanguageBackendContext.GetExtensionFor<T>());
            return _contexts.ContainsKey(fullName);
        }

        public bool HasContext<T>(string name)
        {
            return HasContext<T>(name, out _);
        }

        public ISourceContext GetContext<T>(string name)
        {
            if (HasContext<T>(name, out var fullName))
                return _contexts[fullName];
            throw new ArgumentException($"{fullName} is not found in context");
        }

        public List<(Action<ILanguageSyntaxActionContext> Action, Func<ILanguageSyntaxActionContext, bool> Predicator)> GetPreSyntaxActions(WellKnownSyntax syntax)
        {
            return CurrentLanguageBackendContext.GetPreSyntaxActions(syntax);
        }

        public List<(Action<ILanguageSyntaxActionContext> Action, Func<ILanguageSyntaxActionContext, bool> Predicator)> GetPostSyntaxActions(WellKnownSyntax syntax)
        {
            return CurrentLanguageBackendContext.GetPostSyntaxActions(syntax);
        }

        public string Flush(string name)
        {
            if (_contexts.ContainsKey(name))
                return _contexts[name].ToSourceString();
            throw new InvalidOperationException();
        }

        public List<(string, string)> FlushAll()
        {
            return _contexts.Select(w => (Name: w.Key, Source: Flush(w.Key))).Where(w => !string.IsNullOrWhiteSpace(w.Source)).ToList();
        }
    }
}