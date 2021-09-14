using System;
using System.Collections.Generic;
using System.Linq;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;
using SharpX.Compiler.Models.Plugin;

namespace SharpX.Compiler.Models
{
    internal class AssemblyContext
    {
        private readonly Dictionary<string, ISourceContext> _contexts;
        private readonly SharpXPluginHost _host;
        private string _variant;

        public ISourceContext Default => _contexts["source"];

        public LanguageBackendContext CurrentLanguageBackendContext => (LanguageBackendContext)_host.CurrentLanguageBackendContext!;

        public AssemblyContext(SharpXPluginHost host)
        {
            _host = host;
            _contexts = new Dictionary<string, ISourceContext>();
            _variant = "";

            InitializeContext();
        }

        public void SwitchVariant(string variant)
        {
            _contexts.Clear();
            _variant = variant;

            InitializeContext();
        }

        private void InitializeContext()
        {
            _contexts.Add("source", new DefaultSourceContext());
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
            var replaced = CurrentLanguageBackendContext.GetSourceMappingGeneratorFor<T>().Invoke(new SourceContextMappingGeneratorArgs(_variant, name));
            fullName = $"{replaced}.{CurrentLanguageBackendContext.GetExtensionFor<T>()}";
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

        public IReadOnlyCollection<LanguageBackendContext.WalkerPair> GetProvidedWalkers()
        {
            return CurrentLanguageBackendContext.Walkers;
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