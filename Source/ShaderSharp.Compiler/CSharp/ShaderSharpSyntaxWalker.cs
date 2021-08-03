using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using ShaderSharp.Compiler.Abstractions.Attributes;
using ShaderSharp.Compiler.Extensions;
using ShaderSharp.Compiler.Models;
using ShaderSharp.Compiler.Models.Source;

namespace ShaderSharp.Compiler.CSharp
{
    public class ShaderSharpSyntaxWalker : CSharpSyntaxWalker
    {
        private readonly ProjectContext _context;
        private readonly List<string> _errors;
        private readonly SemanticModel _semanticModel;

        private SourceContext _currentContext;

        public IReadOnlyCollection<string> Errors => _errors.AsReadOnly();

        public ShaderSharpSyntaxWalker(SemanticModel semanticModel, ProjectContext context) : base(SyntaxWalkerDepth.Token)
        {
            _semanticModel = semanticModel;
            _context = context;
            _errors = new List<string>();

            _currentContext = _context.Default;
        }

        #region Top-Level Member Declarations

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var capture = ClassLikeDeclarationCapture.Capture(node, _semanticModel);

            PreProcessClassLikeDeclaration(capture);

            base.VisitClassDeclaration(node);

            PostProcessClassLikeDeclaration(capture);
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            var capture = ClassLikeDeclarationCapture.Capture(node, _semanticModel);

            PreProcessClassLikeDeclaration(capture);

            base.VisitStructDeclaration(node);

            PostProcessClassLikeDeclaration(capture);
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            var capture = ClassLikeDeclarationCapture.Capture(node, _semanticModel);

            PreProcessClassLikeDeclaration(capture);

            base.VisitInterfaceDeclaration(node);

            PostProcessClassLikeDeclaration(capture);
        }

        public override void VisitRecordDeclaration(RecordDeclarationSyntax node)
        {
            var capture = ClassLikeDeclarationCapture.Capture(node, _semanticModel);

            PreProcessClassLikeDeclaration(capture);

            base.VisitRecordDeclaration(node);

            PostProcessClassLikeDeclaration(capture);
        }

        private void PreProcessClassLikeDeclaration(ClassLikeDeclarationCapture capture)
        {
            if (capture.IsNested())
                _errors.Add("ShaderSharp does not support nested type declarations");

            if (capture.HasAttribute<ExportAttribute>())
            {
                var attr = capture.GetAttributeData<ExportAttribute>().AsAttributeInstance<ExportAttribute>();
                if (!string.IsNullOrWhiteSpace(attr.FilePath))
                    _currentContext = _context.HasContext(attr.FilePath) ? _context.GetContext(attr.FilePath) : _context.AddContext(attr.FilePath);
            }

            if (capture.HasAttribute<ComponentAttribute>())
            {
                var attr = capture.GetAttributeData<ComponentAttribute>().AsAttributeInstance<ComponentAttribute>();
                if (string.IsNullOrWhiteSpace(attr.Name) || !attr.IsValidName())
                    _errors.Add("The component name must be ASCII strings that does not contain any whitespaces.");

                _currentContext.OpenStruct(attr.Name);
            }
        }

        private void PostProcessClassLikeDeclaration(ClassLikeDeclarationCapture capture)
        {
            if (capture.HasAttribute<ComponentAttribute>())
                _currentContext.CloseStruct();
        }

        #endregion

        #region Second-Level Member Declarations

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            if (node.Initializer != null)
                _errors.Add("ShaderSharp does not support property initializer");

            if (node.AccessorList?.Accessors != null)
                foreach (var accessor in node.AccessorList?.Accessors)
                    if (accessor.Body != null || accessor.ExpressionBody != null)
                        _errors.Add("ShaderSharp does not support property expression bodies");

            if (node.ExpressionBody != null)
                _errors.Add("ShaderSharp does not support property expression bodies");

            var capture = FieldLikeDeclarationCapture.Capture(node, _semanticModel);

            ProcessFieldLikeDeclaration(capture);

            base.VisitPropertyDeclaration(node);
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            if (node.Declaration.Variables.Count > 1)
                _errors.Add("ShaderSharp does not support multiple declarations on single field declaration");
            if (node.Declaration.Variables.Any(w => w.Initializer != null))
                _errors.Add("ShaderSharp does not support field initializer");

            var capture = FieldLikeDeclarationCapture.Capture(node, _semanticModel);

            base.VisitFieldDeclaration(node);
        }

        private void ProcessFieldLikeDeclaration(FieldLikeDeclarationCapture capture)
        {
            if (capture.HasAttributeOfDeclaration<ExternalAttribute>())
                return;

            if (capture.HasAttributeOfDeclaration<GlobalMemberAttribute>())
            {
                if (!capture.IsStaticMember())
                    _errors.Add("ShaderSharp does not recommend instance member declaration in struct, but worked correctly");

                _currentContext.AddGlobalMember(capture.GetDeclarationType(), capture.GetName());
                return;
            }

            if (capture.IsStaticMember())
            {
                _errors.Add("ShaderSharp does not support static member declaration in struct");
                return;
            }

            if (capture.HasAttributeOfDeclaration<SemanticsAttribute>())
            {
                var semantics = capture.GetAttributeDataOfDeclaration<SemanticsAttribute>().AsAttributeInstance<SemanticsAttribute>();
                var extras = new[]
                {
                    new KeyValuePair<string, string>("Semantics", semantics.Semantics)
                };

                _currentContext.AddStructMember(capture.GetDeclarationType(), capture.GetName(), extras);
            }
            else
            {
                _currentContext.AddStructMember(capture.GetDeclarationType(), capture.GetName());
            }
        }

        #endregion
    }
}