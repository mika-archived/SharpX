using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;
using SharpX.Compiler.Extensions;
using SharpX.Compiler.Models.Plugin;

namespace SharpX.Compiler.Models
{
    internal class CompilationModule
    {
        private readonly List<string> _errors;
        private readonly List<string> _warnings;

        public bool HasErrors => _errors.Count > 0;

        public bool HasWarnings => _warnings.Count > 0;

        public IReadOnlyCollection<string> Errors => _errors.AsReadOnly();

        public IReadOnlyCollection<string> Warnings => _warnings.AsReadOnly();

        public SyntaxTree SyntaxTree { get; private set; }

        public DocumentId Id { get; }

        public CompilationModule(DocumentId id, SyntaxTree tree)
        {
            Id = id;
            SyntaxTree = tree;
            _errors = new List<string>();
            _warnings = new List<string>();
        }

        public void UpdateSyntaxTree(SyntaxTree tree)
        {
            SyntaxTree = tree;
        }

        public void Rewrite(CSharpSyntaxRewriter rewriter)
        {
            SyntaxTree = rewriter.Visit(SyntaxTree.GetRoot()).SyntaxTree;
        }

        public void Compile(SemanticModel model, AssemblyContext context, KeyValuePair<string, string[]> variant)
        {
            if (SyntaxTree.GetDiagnostics().Any(w => w.Severity == DiagnosticSeverity.Error))
            {
                _errors.AddRange(SyntaxTree.GetDiagnostics().Where(w => w.Severity == DiagnosticSeverity.Error).Select(w => w.ToErrorMessage()));
                return;
            }

            CompileByIntegratedWalker(model, context);
            CompileByProvidedWalker(model, context, variant);
        }

        private void CompileByIntegratedWalker(SemanticModel model, AssemblyContext context)
        {
            var walker = new SharpXSyntaxWalker(model, context);

            try
            {
                walker.Visit(SyntaxTree.GetRoot());
            }
            catch (Exception e)
            {
                if (Debugger.IsAttached)
                    Debug.WriteLine(e.Message);
            }
            finally
            {
                _errors.AddRange(walker.Errors);
                _warnings.AddRange(walker.Warnings);
            }
        }

        private void CompileByProvidedWalker(SemanticModel model, AssemblyContext assembly, KeyValuePair<string, string[]> variant)
        {
            if (SyntaxTree.GetRoot() is not CSharpSyntaxNode node)
                return;

            foreach (var (generator, predicator) in assembly.GetProvidedWalkers())
            {
                var context = new LanguageSyntaxWalkerContext( model, assembly.Default, new AddOnlyCollection<IError>(), new AddOnlyCollection<IError>(), assembly);

                try
                {
                    if (predicator == null || predicator.Invoke((variant.Key, variant.Value)))
                    {
                        var walker = generator.Invoke(context);
                        walker.Visit(node);
                    }
                }
                catch (Exception e)
                {
                    if (Debugger.IsAttached)
                        Debug.WriteLine(e.Message);
                }
                finally
                {
                    _errors.AddRange(context.Errors.Select(w => w.GetMessage()));
                    _warnings.AddRange(context.Warnings.Select(w => w.GetMessage()));
                }
            }
        }
    }
}