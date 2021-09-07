using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
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
            _host = new SharpXPluginHost(_options.CustomOptions);
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

        public void LockBuildTarget()
        {
            if (!_host.HasLanguageBackend(_options.Target))
            {
                _errors.Add($"Failed to load language backend implementation of {_options.Target}.");
                return;
            }

            _host.InitializeHostByIdentifier(_options.Target);
        }

        public async Task CompileAsync(ImmutableArray<string> items)
        {
            _errors.Clear();

            var context = new AssemblyContext(_host);
            var backend = context.CurrentLanguageBackendContext;

            const string predefinedPreprocessor = "SHARPX_COMPILER";

            foreach (var variant in backend.PreprocessorVariants)
            {
                context.SwitchVariant(variant.Key);

                var preprocessors = variant.Value.ToList();
                if (!preprocessors.Contains(predefinedPreprocessor))
                    preprocessors.Add(predefinedPreprocessor);

                var modules = new ConcurrentBag<CompilationModule>();
                var projectId = ProjectId.CreateNewId("SharpX.Assembly");
                var workspace = new AdhocWorkspace();
                var solution = workspace.CurrentSolution.AddProject(projectId, "SharpX.Assembly", "SharpX.Assembly", LanguageNames.CSharp)
                                        .WithProjectMetadataReferences(projectId, _references)
                                        .WithProjectParseOptions(projectId, CSharpParseOptions.Default.WithPreprocessorSymbols(preprocessors))
                                        .WithProjectCompilationOptions(projectId, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                foreach (var path in items)
                {
                    var absolute = string.IsNullOrWhiteSpace(_options.ProjectRoot) ? path : Path.Combine(_options.ProjectRoot, path);
                    if (!File.Exists(absolute))
                        throw new FileNotFoundException(path);

                    var source = SourceText.From(await File.ReadAllTextAsync(absolute, Encoding.UTF8).ConfigureAwait(false), Encoding.UTF8);
                    var documentId = DocumentId.CreateNewId(projectId, path);
                    solution = solution.AddDocument(documentId, path, source);

                    var syntax = await solution.GetDocument(documentId)!.GetSyntaxTreeAsync().ConfigureAwait(false);
                    var module = new CompilationModule(documentId, syntax!);

                    modules.Add(module);
                }

                if (modules.Any(w => w.HasErrors))
                {
                    _errors.AddRange(modules.SelectMany(w => w.Errors));
                    return;
                }


                foreach (var rc in _host.AvailableRewriters)
                foreach (var generator in rc.GetGenerators())
                foreach (var module in modules)
                {
                    var project = solution.Projects.First(w => w.Id == projectId);
                    var model = await project.GetDocument(module.Id)!.GetSemanticModelAsync().ConfigureAwait(false);
                    if (model == null)
                        continue;

                    var rewriter = generator.Invoke(new LanguageSyntaxRewriterContext(solution, model));
                    module.Rewrite(rewriter);

                    solution = solution.WithDocumentSyntaxRoot(module.Id, await module.SyntaxTree.GetRootAsync().ConfigureAwait(false));

                    var newSyntaxTree = await solution.Projects.First(w => w.Id == projectId).GetDocument(module.Id)!.GetSyntaxTreeAsync().ConfigureAwait(false);
                    module.UpdateSyntaxTree(newSyntaxTree!);
                }

                foreach (var module in modules)
                {
                    var project = solution.Projects.First(w => w.Id == projectId);
                    var model = await project.GetDocument(module.Id)!.GetSemanticModelAsync().ConfigureAwait(false);
                    if (model == null)
                        continue;

                    module.Compile(model, context, variant);
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

                foreach (var (filename, source) in context.FlushAll())
                {
                    var path = Path.Combine(_options.OutputDir, filename);
                    var basename = Path.GetDirectoryName(path);
                    if (!string.IsNullOrWhiteSpace(basename) && !Directory.Exists(basename))
                        Directory.CreateDirectory(basename);

                    await File.WriteAllTextAsync(path, source).ConfigureAwait(false);
                }
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
            yield return Path.Combine(runtime, "System.Collections.dll");
            yield return Path.Combine(runtime, "System.Collections.Immutable.dll");
            yield return Path.Combine(runtime, "System.Linq.dll");
        }
    }
}