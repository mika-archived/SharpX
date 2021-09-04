using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Text.Json;

using Microsoft.CodeAnalysis.CSharp;

using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.Models.Plugin
{
    internal class LanguageBackendContext : ILanguageBackendContext
    {
        private static readonly Dictionary<string, string[]> DefaultVariant = new() { { "", Array.Empty<string>() } };
        private readonly Dictionary<string, Func<(string, string?[]), bool>> _buildPredicatorMappings;
        private readonly Dictionary<string, string> _extensionsMappings;
        private readonly Dictionary<string, Func<ISourceContextMappingArgs, string>> _fileGeneratorMappings;
        private readonly Dictionary<string, Func<ISourceContextGeneratorArgs, ISourceContext>> _generatorMappings;
        private readonly Dictionary<string, string[]> _preprocessorVariants;
        private readonly List<WalkerPair> _walkers;

        public IReadOnlyCollection<WalkerPair> Walkers => _walkers.AsReadOnly();

        public IReadOnlyDictionary<string, string[]> PreprocessorVariants => _preprocessorVariants.Count == 0 ? DefaultVariant : _preprocessorVariants;

        public LanguageBackendContext(JsonElement extraOptions, IsolatedAssemblyLoadContext context)
        {
            _buildPredicatorMappings = new Dictionary<string, Func<(string, string?[]), bool>>();
            _extensionsMappings = new Dictionary<string, string>();
            _fileGeneratorMappings = new Dictionary<string, Func<ISourceContextMappingArgs, string>>();
            _generatorMappings = new Dictionary<string, Func<ISourceContextGeneratorArgs, ISourceContext>>();
            _preprocessorVariants = new Dictionary<string, string[]>();
            _walkers = new List<WalkerPair>();
            ExtraOptions = extraOptions;
            LoadContext = context;
        }

        public JsonElement ExtraOptions { get; }

        public AssemblyLoadContext LoadContext { get; }

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

        public void RegisterCSharpSyntaxWalker(Func<ILanguageSyntaxWalkerContext, CSharpSyntaxWalker> generator, Func<(string, string?[]), bool>? doBuild = null)
        {
            _walkers.Add(new WalkerPair(generator, doBuild));
        }

        public void RegisterCompilationVariants(string key, string?[] preprocessors)
        {
            if (_preprocessorVariants.ContainsKey(key))
                return;

            _preprocessorVariants.Add(key, preprocessors.Where(w => w != null).Cast<string>().ToArray());
        }

        public void ShouldBuildForThisVariant(Func<(string, string?[]), bool> predicate)
        {
            _buildPredicatorMappings.Add("*", predicate);
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

        internal record WalkerPair(Func<ILanguageSyntaxWalkerContext, CSharpSyntaxWalker> Walker, Func<(string, string?[]), bool>? Predicator) { }
    }
}