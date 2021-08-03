using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

using ShaderSharp.Compiler.CSharp;
using ShaderSharp.Compiler.Extensions;

namespace ShaderSharp.Compiler.Models
{
    public class CompilationModule
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

        public void Compile(SemanticModel model, ProjectContext context)
        {
            if (SyntaxTree.GetDiagnostics().Any(w => w.Severity == DiagnosticSeverity.Error))
            {
                _errors.AddRange(SyntaxTree.GetDiagnostics().Where(w => w.Severity == DiagnosticSeverity.Error).Select(w => w.ToErrorMessage()));
                return;
            }

            var walker = new ShaderSharpSyntaxWalker(model, context);
            walker.Visit(SyntaxTree.GetRoot());

            _errors.AddRange(walker.Errors);
        }
    }
}