using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.CodeAnalysis;

using SharpX.Compiler.Extensions;

namespace SharpX.Compiler.Models
{
    internal class CompilationModule
    {
        private readonly List<string> _errors;

        public bool HasErrors => _errors.Count > 0;

        public IReadOnlyCollection<string> Errors => _errors.AsReadOnly();

        public SyntaxTree SyntaxTree { get; }

        public CompilationModule(SyntaxTree tree)
        {
            SyntaxTree = tree;
            _errors = new List<string>();
        }

        public void Compile(SemanticModel model, AssemblyContext context)
        {
            if (SyntaxTree.GetDiagnostics().Any(w => w.Severity == DiagnosticSeverity.Error))
            {
                _errors.AddRange(SyntaxTree.GetDiagnostics().Where(w => w.Severity == DiagnosticSeverity.Error).Select(w => w.ToErrorMessage()));
                return;
            }

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
            }
        }
    }
}