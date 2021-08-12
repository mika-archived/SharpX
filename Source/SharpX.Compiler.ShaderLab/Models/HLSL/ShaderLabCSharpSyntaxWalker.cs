using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Enums;
using SharpX.Compiler.Composition.Interfaces;
using SharpX.Compiler.Extensions;
using SharpX.Compiler.ShaderLab.Models.HLSL.Captures;
using SharpX.Compiler.ShaderLab.Models.HLSL.Declarators;
using SharpX.Compiler.ShaderLab.Models.HLSL.Statements;
using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Compiler.ShaderLab.Models.HLSL
{
    public class ShaderLabCSharpSyntaxWalker : CSharpSyntaxWalker
    {
        private readonly ILanguageSyntaxWalkerContext _context;

        public WellKnownSyntax? CurrentCapturing => CapturingStack.Count > 0 ? CapturingStack.Peek() : null;

        public Stack<WellKnownSyntax> CapturingStack { get; }

        public INestableStatement? Statement { get; internal set; }

        public ShaderLabCSharpSyntaxWalker(ILanguageSyntaxWalkerContext context) : base(SyntaxWalkerDepth.Token)
        {
            _context = context;
            CapturingStack = new Stack<WellKnownSyntax>();
        }

        public override void DefaultVisit(SyntaxNode node)
        {
            base.DefaultVisit(node);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            VisitTypeDeclaration(node);
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            VisitTypeDeclaration(node);
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            VisitTypeDeclaration(node);
        }

        public override void VisitRecordDeclaration(RecordDeclarationSyntax node)
        {
            VisitTypeDeclaration(node);
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            var context = _context.SourceContext.OfType<ShaderLabHLSLSourceContext>();
            if (!_context.IsContextOpened() || context == null)
                return;

            if (node.Initializer != null)
                _context.Warnings.Add(new DefaultError(node.Initializer, "SharpX.ShaderLab Compiler does not support property initializers"));

            if (node.AccessorList != null)
                foreach (var accessor in node.AccessorList.Accessors.Where(w => w.Body != null || w.ExpressionBody != null))
                    _context.Warnings.Add(new DefaultError(accessor, "SharpX.ShaderLab Compiler does not support property bodies / expression bodies in set/get accessors"));

            if (node.ExpressionBody != null)
                _context.Warnings.Add(new DefaultError(node.ExpressionBody, "SharpX.ShaderLab Compiler does not support property expression bodies in get accessor"));

            if (node.HasAttribute<ExternalAttribute>(_context.SemanticModel))
                return; // skipped to transpile

            var capture = new PropertyDeclarationDeclarator(node, _context.SemanticModel);

            if (capture.HasAttribute<GlobalMemberAttribute>())
            {
                if (!node.HasModifiers(SyntaxKind.StaticKeyword))
                    _context.Warnings.Add(new DefaultError(node, "SharpX.ShaderLab Compiler recommended to declare global member as static properties or fields"));

                context.AddGlobalMember(capture.GetDeclaredType(), capture.GetIdentifierName());
                return;
            }

            if (context.StructDeclaration == null)
            {
                _context.Warnings.Add(new DefaultError(node, "Property declaration found outside of structure definition"));
                return;
            }

            if (capture.HasAttribute<SemanticAttribute>())
            {
                if (node.HasModifiers(SyntaxKind.StaticKeyword))
                    _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not process semantic members to be declared as static modifier"));

                var attr = capture.GetAttribute<SemanticAttribute>()!;
                if (attr.IsValidSemantics())
                {
                    context.StructDeclaration.AddMember(capture.GetDeclaredType(), capture.GetIdentifierName(), attr.Semantic);
                    return;
                }
            }

            _context.Warnings.Add(new DefaultError(node, "The SharpX.ShaderLab compiler will transpile without SEMANTIC specification, but this may cause the ShaderLab compiler to throw an error "));
            context.StructDeclaration.AddMember(capture.GetDeclaredType(), capture.GetIdentifierName(), null);
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            var context = _context.SourceContext.OfType<ShaderLabHLSLSourceContext>();
            if (!_context.IsContextOpened() || context == null)
                return;

            if (node.Declaration.Variables.Count > 1)
                _context.Errors.Add(new DefaultError(node.Declaration, "SharpX.ShaderLab Compiler does not support multiple declarations on single field declaration"));
            if (node.Declaration.Variables.Any(w => w.Initializer != default))
                _context.Warnings.Add(new DefaultError(node.Declaration, "SharpX.ShaderLab Compiler does not support field initializers"));

            if (node.HasAttribute<ExternalAttribute>(_context.SemanticModel))
                return; // skipped to transpile

            var capture = new FieldDeclarationDeclarator(node, _context.SemanticModel);

            if (capture.HasAttribute<GlobalMemberAttribute>())
            {
                if (!node.HasModifiers(SyntaxKind.StaticKeyword))
                    _context.Warnings.Add(new DefaultError(node, "SharpX.ShaderLab Compiler recommended to declare global member as static properties or fields"));

                context.AddGlobalMember(capture.GetDeclaredType(), capture.GetIdentifierName());
                return;
            }

            if (context.StructDeclaration == null)
            {
                _context.Warnings.Add(new DefaultError(node, "Field declaration found outside of structure definition"));
                return;
            }

            if (capture.HasAttribute<SemanticAttribute>())
            {
                if (node.HasModifiers(SyntaxKind.StaticKeyword))
                    _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not process semantic members to be declared as static modifier"));

                var attr = capture.GetAttribute<SemanticAttribute>()!;
                if (attr.IsValidSemantics())
                {
                    context.StructDeclaration.AddMember(capture.GetDeclaredType(), capture.GetIdentifierName(), attr.Semantic);
                    return;
                }
            }

            _context.Warnings.Add(new DefaultError(node, "The SharpX.ShaderLab compiler will transpile without SEMANTIC specification, but this may cause the ShaderLab compiler to throw an error "));
            context.StructDeclaration.AddMember(capture.GetDeclaredType(), capture.GetIdentifierName(), null);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var declarator = new MethodDeclarationDeclarator(node, _context.SemanticModel);

            if (declarator.HasAttribute<ExternalAttribute>())
                return; // skipped to transpile

            var context = _context.SourceContext.OfType<ShaderLabHLSLSourceContext>();
            if (context == null)
                return;

            context.OpenFunction(declarator.GetIdentifierName(), declarator.GetDeclaredReturnType());

            using (SyntaxCaptureScope.Create(this, WellKnownSyntax.MethodDeclarationSyntax))
            {
                Visit(node.ParameterList);

                if (node.Body != null)
                    Visit(node.Body);

                if (node.ExpressionBody != null)
                    using (SyntaxCaptureScope.Create(this, WellKnownSyntax.BlockSyntax))
                    {
                        context.FunctionDeclaration.AddSourcePart(new Block());
                        Visit(node.ExpressionBody);
                    }
            }

            context.CloseFunction();
        }

        public override void VisitParameter(ParameterSyntax node)
        {
            if (node.Type == null)
            {
                _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler requires the type to be specified for parameters"));
                return;
            }

            var capture = TypeDeclarationCapture.Capture(node.Type, _context.SemanticModel);

            if (CurrentCapturing == WellKnownSyntax.MethodDeclarationSyntax)
                _context.SourceContext.OfType<ShaderLabHLSLSourceContext>()?.FunctionDeclaration!.AddArgument(capture.GetActualName(), node.Identifier.ValueText);
        }

        public override void VisitBlock(BlockSyntax node)
        {
            if (CurrentCapturing == WellKnownSyntax.MethodDeclarationSyntax)
                using (SyntaxCaptureScope.Create(this, WellKnownSyntax.BlockSyntax))
                {
                    _context.SourceContext.OfType<ShaderLabHLSLSourceContext>()?.FunctionDeclaration!.AddSourcePart(new Block());

                    foreach (var statement in node.Statements)
                        Visit(statement);
                }
        }

        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
            var declaration = _context.SourceContext.OfType<ShaderLabHLSLSourceContext>()?.FunctionDeclaration;
            if (declaration == null)
                return;

            if (declaration.PopLastSourcePartIfAvailable<Block>(out var block))
                using (var scope = SyntaxCaptureScope<ReturnStatement>.Create(this, WellKnownSyntax.ReturnStatementSyntax, new ReturnStatement()))
                {
                    Visit(node.Expression);
                    block.AddSourcePart(scope.Statement);
                }
        }

        public override void VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            var declaration = _context.SourceContext.OfType<ShaderLabHLSLSourceContext>()?.FunctionDeclaration;
            if (declaration == null)
                return;

            if (CurrentCapturing == WellKnownSyntax.ReturnStatementSyntax)
                Statement!.AddSourcePart(new Span(node.ToFullString()));
        }

        private void VisitTypeDeclaration(TypeDeclarationSyntax node)
        {
            var declarator = TypeDeclarationDeclarator.Create(node, _context.SemanticModel);
            if (declarator.IsNestedDeclaration())
            {
                _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support nested type declarations"));
                return;
            }

            if (declarator.HasAttribute<ExternalAttribute>())
                return; // skipped to transpile

            if (declarator.HasAttribute<ExportAttribute>())
            {
                var attr = declarator.GetAttribute<ExportAttribute>();
                if (attr!.IsValidSourceName())
                {
                    _context.CreateOrGetContext(attr.Source);
                }
                else
                {
                    _context.Errors.Add(new DefaultError(node, "The value specified for ComponentAttribute must be a valid file name"));
                    return;
                }
            }

            if (!_context.IsContextOpened())
                return;

            var isStruct = declarator.HasAttribute<ComponentAttribute>();
            if (isStruct)
            {
                var attr = declarator.GetAttribute<ComponentAttribute>();
                _context.SourceContext.OfType<ShaderLabHLSLSourceContext>()?.OpenStruct(string.IsNullOrWhiteSpace(attr?.Name) ? declarator.GetDeclarationName() : attr.Name);
            }

            foreach (var member in node.Members)
                Visit(member);

            if (isStruct)
            {
                var context = _context.SourceContext.OfType<ShaderLabHLSLSourceContext>();
                if (context == null)
                    return; // UNREACHABLE

                void RecursiveExpandProperties(INamedTypeSymbol symbol)
                {
                    foreach (var member in symbol.GetMembers())
                        switch (member)
                        {
                            case IPropertySymbol s:
                                var propertyCapture = new PropertySymbolCapture(s, _context.SemanticModel);
                                context.StructDeclaration?.AddMember(propertyCapture.GetDeclaredType(), propertyCapture.GetIdentifierName(), propertyCapture.GetSemanticsName());
                                break;

                            case IFieldSymbol s:
                                if (s.Name.Contains("k__BackingField"))
                                    break;

                                var fieldCapture = new FieldSymbolCapture(s, _context.SemanticModel);
                                context.StructDeclaration?.AddMember(fieldCapture.GetDeclaredType(), fieldCapture.GetIdentifierName(), fieldCapture.GetSemanticsName());
                                break;
                        }

                    if (symbol.BaseType != null)
                        RecursiveExpandProperties(symbol.BaseType);
                    if (symbol.Interfaces.Any())
                        symbol.Interfaces.ForEach(RecursiveExpandProperties);
                }

                if (node.BaseList != null)
                {
                    var symbol = _context.SemanticModel.GetDeclaredSymbol(node);
                    if (symbol?.BaseType != null)
                        RecursiveExpandProperties(symbol.BaseType);
                    if (symbol?.Interfaces.Any() == true)
                        symbol.Interfaces.ForEach(RecursiveExpandProperties);
                }

                if (CurrentCapturing == null)
                    _context.SourceContext.OfType<ShaderLabHLSLSourceContext>()?.CloseStruct();
            }
        }

        #region Unsupported Syntaxes

        public override void VisitAwaitExpression(AwaitExpressionSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support async-await expressions"));
        }

        public override void VisitTryStatement(TryStatementSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support try-catch(-finally) statements"));
        }

        public override void VisitCatchClause(CatchClauseSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support try-catch(-finally) statements"));
        }

        public override void VisitCatchDeclaration(CatchDeclarationSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support try-catch(-finally) statements"));
        }

        public override void VisitCatchFilterClause(CatchFilterClauseSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support try-catch(-finally) statements"));
        }

        public override void VisitFinallyClause(FinallyClauseSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support try-catch(-finally) statements"));
        }

        #endregion
    }
}