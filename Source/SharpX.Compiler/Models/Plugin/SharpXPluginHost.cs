using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Attributes;
using SharpX.Compiler.Composition.Interfaces;
using SharpX.Compiler.Extensions;

namespace SharpX.Compiler.Models.Plugin
{
    internal class SharpXPluginHost
    {
        private readonly ImmutableDictionary<string, JsonElement> _extraOptions;
        private readonly Dictionary<string, (ILanguageBackend, ILanguageBackendContext)> _implementations;
        private readonly List<KeyValuePair<string, (ISourceRewriter, ISourceRewriterContext)>> _rewriters;
        private string? _identifier;

        public ILanguageBackend? CurrentLanguageBackend
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_identifier))
                    return null;
                if (_implementations.ContainsKey(_identifier))
                    return _implementations[_identifier].Item1;
                return null;
            }
        }

        public ILanguageBackendContext? CurrentLanguageBackendContext
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_identifier))
                    return null;
                if (_implementations.ContainsKey(_identifier))
                    return _implementations[_identifier].Item2;
                return null;
            }
        }

        public List<SourceRewriterContext> AvailableRewriters
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_identifier))
                    return new List<SourceRewriterContext>();
                return _rewriters.Where(w => w.Key == _identifier || w.Key == "*")
                                 .Select(w => w.Value.Item2)
                                 .Cast<SourceRewriterContext>()
                                 .Where(w => w != null)
                                 .ToList();
            }
        }

        public SharpXPluginHost(ImmutableDictionary<string, JsonElement> extraOptions)
        {
            _extraOptions = extraOptions;
            _implementations = new Dictionary<string, (ILanguageBackend, ILanguageBackendContext)>();
            _rewriters = new List<KeyValuePair<string, (ISourceRewriter, ISourceRewriterContext)>>();
            _identifier = null;
        }

        public bool LoadPluginAtPath(string path)
        {
            try
            {
                var context = new SharpXPluginContext(path);
                var assembly = context.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
                foreach (var type in assembly.GetTypes())
                    switch (type)
                    {
                        case { } when type.GetCustomAttribute<LanguageBackendAttribute>() != null && typeof(ILanguageBackend).IsAssignableFrom(type):
                            LoadLanguageBackendImplementation(type, path);
                            break;

                        case { } when type.GetCustomAttribute<SourceRewriterAttribute>() != null && typeof(ISourceRewriter).IsAssignableFrom(type):
                            LoadSourceRewriterImplementation(type);
                            break;
                    }

                return true;
            }
            catch (Exception e)
            {
                if (Debugger.IsAttached)
                    Debug.WriteLine(e.Message);

                return false;
            }
        }

        private void LoadLanguageBackendImplementation(Type t, string path)
        {
            if (Activator.CreateInstance(t) is not ILanguageBackend backend)
                return; // ???

            var options = _extraOptions.ContainsKey(backend.Identifier) ? _extraOptions[backend.Identifier] : new JsonElement();
            var context = new IsolatedAssemblyLoadContext(path);
            _implementations.Add(backend.Identifier, (backend, new LanguageBackendContext(options, context)));
        }

        private void LoadSourceRewriterImplementation(Type t)
        {
            if (Activator.CreateInstance(t) is not ISourceRewriter rewriter)
                return;

            foreach (var identifier in rewriter.SupportedIdentifiers)
            {
                var options = _extraOptions.ContainsKey(rewriter.Identifier) ? _extraOptions[rewriter.Identifier] : new JsonElement();
                _rewriters.Add(new KeyValuePair<string, (ISourceRewriter, ISourceRewriterContext)>(identifier, (rewriter, new SourceRewriterContext(options))));
            }
        }

        public bool HasLanguageBackend(string identifier)
        {
            return _implementations.ContainsKey(identifier);
        }

        public void InitializeHostByIdentifier(string identifier)
        {
            InitializeLanguageBackend(identifier);
            InitializeSourceRewriter(identifier);
            SwitchLanguageBackend(identifier);
        }

        private void InitializeLanguageBackend(string identifier)
        {
            if (!_implementations.ContainsKey(identifier))
                throw new InvalidOperationException();

            var (implementation, context) = _implementations[identifier];
            implementation.Initialize(context);

            SwitchLanguageBackend(identifier);
        }

        private void InitializeSourceRewriter(string identifier)
        {
            if (_rewriters.None(w => w.Key == identifier || w.Key == "*"))
                return;

            var rewriters = _rewriters.Where(w => w.Key == identifier || w.Key == "*").Select(w => w.Value);
            rewriters.ForEach(w => w.Item1.Initialize(w.Item2));
        }

        [MemberNotNull(nameof(_identifier))]
        public void SwitchLanguageBackend(string identifier)
        {
            if (!_implementations.ContainsKey(identifier))
                throw new InvalidOperationException();

            _identifier = identifier;
        }
    }
}