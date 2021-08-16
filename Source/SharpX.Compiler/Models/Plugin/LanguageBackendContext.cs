using System;
using System.Collections.Generic;
using System.Text.Json;

using Microsoft.CodeAnalysis.CSharp;

using SharpX.Compiler.Composition.Enums;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.Models.Plugin
{
    internal class LanguageBackendContext : ILanguageBackendContext
    {
        private static readonly Func<ILanguageSyntaxActionContext, bool> DefaultPredicator = _ => true;
        private readonly Dictionary<WellKnownSyntax, List<(Action<ILanguageSyntaxActionContext> Action, Func<ILanguageSyntaxActionContext, bool> Predicator)>> _afterActions;
        private readonly Dictionary<WellKnownSyntax, List<(Action<ILanguageSyntaxActionContext> Action, Func<ILanguageSyntaxActionContext, bool> Predicator)>> _beforeActions;
        private readonly Dictionary<string, string> _extensionsMappings;
        private readonly Dictionary<string, Func<ISourceContextGeneratorArgs, ISourceContext>> _generatorMappings;
        private readonly List<Func<ILanguageSyntaxWalkerContext, CSharpSyntaxWalker>> _walkers;

        public IReadOnlyCollection<Func<ILanguageSyntaxWalkerContext, CSharpSyntaxWalker>> Walkers => _walkers.AsReadOnly();

        public LanguageBackendContext(JsonElement extraOptions)
        {
            _afterActions = new Dictionary<WellKnownSyntax, List<(Action<ILanguageSyntaxActionContext> Action, Func<ILanguageSyntaxActionContext, bool> Predicator)>>();
            _beforeActions = new Dictionary<WellKnownSyntax, List<(Action<ILanguageSyntaxActionContext> Action, Func<ILanguageSyntaxActionContext, bool> Predicator)>>();
            _extensionsMappings = new Dictionary<string, string>();
            _generatorMappings = new Dictionary<string, Func<ISourceContextGeneratorArgs, ISourceContext>>();
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