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

        public void UpdateSyntaxTree(SyntaxTree? tree)
        {
            if (tree == null)
                return;

            SyntaxTree = tree;
        }

        public void Rewrite(CSharpSyntaxRewriter rewriter)
        {
            SyntaxTree = rewriter.Visit(SyntaxTree.GetRoot()).SyntaxTree;
        }

        public void Compile(Compilation compilation, SemanticModel model, AssemblyContext context)
        {
            if (SyntaxTree.GetDiagnostics().Any(w => w.Severity == DiagnosticSeverity.Error))
            {
                _errors.AddRange(SyntaxTree.GetDiagnostics().Where(w => w.Severity == DiagnosticSeverity.Error).Select(w => w.ToErrorMessage()));
                return;
            }

            CompileByIntegratedWalker(compilation, model, context);
            CompileByProvidedWalker(compilation, model, context);
        }

        private void CompileByIntegratedWalker(Compilation compilation, SemanticModel model, AssemblyContext context)
        {
            var walker = new SharpXSyntaxWalker(compilation, model, context);

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

        private void CompileByProvidedWalker(Compilation compilation, SemanticModel model, AssemblyContext assembly)
        {
            if (SyntaxTree.GetRoot() is not CSharpSyntaxNode node)
                return;

            foreach (var generator in assembly.GetProvidedWalkers())
            {
                var context = new LanguageSyntaxWalkerContext(compilation, model, assembly.Default, new AddOnlyCollection<IError>(), new AddOnlyCollection<IError>(), assembly);

                try
                {
                    var walker = generator.Invoke(context);
                    walker.Visit(node);
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