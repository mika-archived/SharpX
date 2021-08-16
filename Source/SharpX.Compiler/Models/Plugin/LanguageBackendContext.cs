using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

using Microsoft.CodeAnalysis.CSharp;

using SharpX.Compiler.Composition.Enums;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.Models.Plugin
{
    internal class LanguageBackendContext : ILanguageBackendContext
    {
        private static readonly Func<ILanguageSyntaxActionContext, bool> DefaultPredicator = _ => true;
        private static readonly Dictionary<string, string[]> DefaultVariant = new() { { "", Array.Empty<string>() } };
        private readonly Dictionary<WellKnownSyntax, List<(Action<ILanguageSyntaxActionContext> Action, Func<ILanguageSyntaxActionContext, bool> Predicator)>> _afterActions;
        private readonly Dictionary<WellKnownSyntax, List<(Action<ILanguageSyntaxActionContext> Action, Func<ILanguageSyntaxActionContext, bool> Predicator)>> _beforeActions;
        private readonly Dictionary<string, string> _extensionsMappings;
        private readonly Dictionary<string, Func<ISourceContextMappingArgs, string>> _fileGeneratorMappings;
        private readonly Dictionary<string, Func<ISourceContextGeneratorArgs, ISourceContext>> _generatorMappings;
        private readonly Dictionary<string, string[]> _preprocessorVariants;
        private readonly List<Func<ILanguageSyntaxWalkerContext, CSharpSyntaxWalker>> _walkers;

        public IReadOnlyCollection<Func<ILanguageSyntaxWalkerContext, CSharpSyntaxWalker>> Walkers => _walkers.AsReadOnly();

        public IReadOnlyDictionary<string, string[]> PreprocessorVariants => _preprocessorVariants.Count == 0 ? DefaultVariant : _preprocessorVariants;

        public LanguageBackendContext(JsonElement extraOptions)
        {
            _afterActions = new Dictionary<WellKnownSyntax, List<(Action<ILanguageSyntaxActionContext> Action, Func<ILanguageSyntaxActionContext, bool> Predicator)>>();
            _beforeActions = new Dictionary<WellKnownSyntax, List<(Action<ILanguageSyntaxActionContext> Action, Func<ILanguageSyntaxActionContext, bool> Predicator)>>();
            _extensionsMappings = new Dictionary<string, string>();
            _fileGeneratorMappings = new Dictionary<string, Func<ISourceContextMappingArgs, string>>();
            _generatorMappings = new Dictionary<string, Func<ISourceContextGeneratorArgs, ISourceContext>>();
            _preprocessorVariants = new Dictionary<string, string[]>();
            _walkers = new List<Func<ILanguageSyntaxWalkerContext, CSharpSyntaxWalker>>();
            ExtraOptions = extraOptions;
        }

        public JsonElement ExtraOptions { get; }

        public void RegisterExtension(string extension)
        {
            _extensionsMappings.Add("*", extension);
        }

        public void RegisterExtensionFor(Type t, string extension)
        {
            _extensionsMappings.Add(t.FullName ?? throw new ArgumentNullException(nameof(t)), extension);
        }

        public void RegisterSourceContextGenerator(Func<ISourceContextGeneratorArgs, ISourceContext> generator)
        {
            _generatorMappings.Add("*", generator);
        }

        public void RegisterSourceContextGeneratorFor(Type t, Func<ISourceContextGeneratorArgs, ISourceContext> generator)
        {
            _generatorMappings.Add(t.FullName ?? throw new ArgumentNullException(nameof(t)), generator);
        }

        public void RegisterSourceContextFileMappingGenerator(Func<ISourceContextMappingArgs, string> generator)
        {
            _fileGeneratorMappings.Add("*", generator);
        }

        public void RegisterSourceContextFileMappingGeneratorFor(Type t, Func<ISourceContextMappingArgs, string> generator)
        {
            _fileGeneratorMappings.Add(t.FullName ?? throw new ArgumentNullException(nameof(t)), generator);
        }

        public void RegisterPreSyntaxAction(WellKnownSyntax syntax, Action<ILanguageSyntaxActionContext> action, Func<ILanguageSyntaxActionContext, bool>? predicate = null)
        {
            if (_beforeActions.ContainsKey(syntax))
                _beforeActions[syntax].Add((action, predicate ?? DefaultPredicator));
            else
                _beforeActions.Add(syntax, new List<(Action<ILanguageSyntaxActionContext> Action, Func<ILanguageSyntaxActionContext, bool> Predicator)> { (action, predicate ?? DefaultPredicator) });
        }

        public void RegisterPostSyntaxAction(WellKnownSyntax syntax, Action<ILanguageSyntaxActionContext> action, Func<ILanguageSyntaxActionContext, bool>? predicate = null)
        {
            if (_afterActions.ContainsKey(syntax))
                _afterActions[syntax].Add((action, predicate ?? DefaultPredicator));
            else
                _afterActions.Add(syntax, new List<(Action<ILanguageSyntaxActionContext> Action, Func<ILanguageSyntaxActionContext, bool> Predicator)> { (action, predicate ?? DefaultPredicator) });
        }

        public void RegisterCSharpSyntaxWalker(Func<ILanguageSyntaxWalkerContext, CSharpSyntaxWalker> generator)
        {
            _walkers.Add(generator);
        }

        public void RegisterCompilationVariants(string key, string?[] preprocessors)
        {
            if (_preprocessorVariants.ContainsKey(key))
                return;

            _preprocessorVariants.Add(key, preprocessors.Where(w => w != null).Cast<string>().ToArray());
        }

        public string GetExtensionFor<T>()
        {
            return GetExtensionFor(typeof(T));
        }

        public string GetExtensionFor(Type t)
        {
            if (_extensionsMappings.ContainsKey(t.FullName ?? throw new ArgumentNullException(nameof(t))))
                return _extensionsMappings[t.FullName];
            return _extensionsMappings["*"];
        }

        public Func<ISourceContextGeneratorArgs, ISourceContext> GetSourceGeneratorFor<T>()
        {
            return GetSourceGeneratorFor(typeof(T));
        }

        public Func<ISourceContextGeneratorArgs, ISourceContext> GetSourceGeneratorFor(Type t)
        {
            if (_generatorMappings.ContainsKey(t.FullName ?? throw new ArgumentNullException(nameof(t))))
                return _generatorMappings[t.FullName];
            return _generatorMappings["*"];
        }

        public Func<ISourceContextMappingArgs, string> GetSourceMappingGeneratorFor<T>()
        {
            return GetSourceMappingGeneratorFor(typeof(T));
        }

        public Func<ISourceContextMappingArgs, string> GetSourceMappingGeneratorFor(Type t)
        {
            if (_fileGeneratorMappings.ContainsKey(t.FullName ?? throw new ArgumentNullException(nameof(t))))
                return _fileGeneratorMappings[t.FullName];
            return _fileGeneratorMappings["*"];
        }

        public List<(Action<ILanguageSyntaxActionContext> Action, Func<ILanguageSyntaxActionContext, bool> Predicator)> GetPreSyntaxActions(WellKnownSyntax syntax)
        {
            if (_beforeActions.ContainsKey(syntax))
                return _beforeActions[syntax];
            return new List<(Action<ILanguageSyntaxActionContext> Action, Func<ILanguageSyntaxActionContext, bool> Predicator)>();
        }

        public List<(Action<ILanguageSyntaxActionContext> Action, Func<ILanguageSyntaxActionContext, bool> Predicator)> GetPostSyntaxActions(WellKnownSyntax syntax)
        {
            if (_afterActions.ContainsKey(syntax))
                return _afterActions[syntax];
            return new List<(Action<ILanguageSyntaxActionContext> Action, Func<ILanguageSyntaxActionContext, bool> Predicator)>();
        }
    }
}