using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

using SharpX.Compiler.Models;
using SharpX.Compiler.Models.Plugin;

namespace SharpX.Compiler
{
    public class SharpXCompiler
    {
        private readonly List<string> _errors;
        private readonly SharpXPluginHost _host;
        private readonly SharpXCompilerOptions _options;
        private readonly List<MetadataReference> _references;
        private readonly List<string> _warnings;

        public IReadOnlyCollection<string> Errors => _errors.AsReadOnly();

        public IReadOnlyCollection<string> Warnings => _warnings.AsReadOnly();

        public SharpXCompiler(SharpXCompilerOptions options)
        {
            _options = options;
            _errors = new List<string>();
            _warnings = new List<string>();
            _references = new List<MetadataReference>();
            _host = new SharpXPluginHost();
        }

        public void LockReferences()
        {
            _references.Clear();

            // internal references
            foreach (var reference in InternalReferences())
                _references.Add(MetadataReference.CreateFromFile(reference));

            // optional references
            foreach (var reference in _options.References)
            {
                if (!File.Exists(reference))
                    throw new FileNotFoundException(reference);

                _references.Add(MetadataReference.CreateFromFile(reference));
            }
        }

        public void LoadPluginModules()
        {
            foreach (var path in _options.Plugins)
            {
                if (!File.Exists(path))
                    throw new FileNotFoundException();

                if (!_host.LoadPluginAtPath(path))
                    throw new InvalidOperationException("Failed to load plugin to context");
            }
        }

        public void Compile()
        {
            _errors.Clear();

            var modules = new ConcurrentBag<CompilationModule>();

            Parallel.ForEach(_options.Items, path =>
            {
                if (!File.Exists(path))
                    throw new FileNotFoundException();

                var source = SourceText.From(File.ReadAllText(path, Encoding.UTF8), Encoding.UTF8);
                var module = new CompilationModule(CSharpSyntaxTree.ParseText(source, CSharpParseOptions.Default, Path.Combine(Environment.CurrentDirectory, path)));

                modules.Add(module);
            });

            if (modules.Any(w => w.HasErrors))
            {
                _errors.AddRange(modules.SelectMany(w => w.Errors));
                return;
            }

            if (!_host.HasLanguageBackend(_options.Target))
            {
                _errors.Add($"Failed to load language backend implementation of {_options.Target}.");
                return;
            }

            _host.InitializeLanguageBackend(_options.Target);

            var compilation = CSharpCompilation.Create("SharpX.Assembly", modules.Select(w => w.SyntaxTree), _references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            var context = new AssemblyContext(_host);

            foreach (var module in modules)
            {
                var model = compilation.GetSemanticModel(module.SyntaxTree);
                module.Compile(model, context);
            }

            if (modules.Any(w => w.HasErrors))
            {
                _errors.AddRange(modules.SelectMany(w => w.Errors));
                return;
            }

            if (modules.Any(w => w.HasWarnings))
                _warnings.AddRange(modules.SelectMany(w => w.Warnings));

            if (!Directory.Exists(_options.OutputDir))
                Directory.CreateDirectory(_options.OutputDir);

            foreach (var (name, source) in context.FlushAll())
            {
                var path = Path.Combine(_options.OutputDir, name);
                File.WriteAllText(path, source);
            }
        }

        private static IEnumerable<string> InternalReferences()
        {
            var runtime = Path.GetDirectoryName(typeof(object).Assembly.Location);
            if (string.IsNullOrWhiteSpace(runtime))
                throw new InvalidOperationException("Failed to get the parent directory of System.Private.CoreLib.dll.");

            yield return Path.Combine(runtime, "mscorlib.dll");
            yield return Path.Combine(runtime, "System.dll");
            yield return Path.Combine(runtime, "System.Core.dll");
            yield return Path.Combine(runtime, "System.Runtime.dll");
            yield return Path.Combine(runtime, "System.Private.CoreLib.dll");
        }
    }
}