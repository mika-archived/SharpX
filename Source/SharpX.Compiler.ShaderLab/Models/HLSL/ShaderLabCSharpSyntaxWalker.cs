using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Enums;
using SharpX.Compiler.Composition.Interfaces;
using SharpX.Compiler.Extensions;
using SharpX.Compiler.ShaderLab.Extensions;
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

        public Stack<INestableStatement> StatementStack { get; }

        public INestableStatement? Statement => StatementStack.Count > 0 ? StatementStack.Peek() : null;

        public ShaderLabCSharpSyntaxWalker(ILanguageSyntaxWalkerContext context) : base(SyntaxWalkerDepth.Token)
        {
            _context = context;
            CapturingStack = new Stack<WellKnownSyntax>();
            StatementStack = new Stack<INestableStatement>();
        }

        public override void DefaultVisit(SyntaxNode node)
        {
            base.DefaultVisit(node);
        }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            var reference = _context.SemanticModel.GetSymbolInfo(node);
            switch (reference.Symbol)
            {
                case ITypeSymbol type:
                    ProcessInclude(TypeDeclarationCapture.Capture(type, _context.SemanticModel));
                    break;

                case INamespaceOrTypeSymbol:
                    break;

                case IParameterSymbol parameter:
                {
                    Statement?.AddSourcePart(new Span(parameter.Name));
                    break;
                }

                case IPropertySymbol property:
                {
                    var capture = new PropertySymbolCapture(property, _context.SemanticModel);

                    if (CapturingStack.Contains(WellKnownSyntax.InitializerExpressionSyntax))
                        Statement?.AddSourcePart(new Span("_auto_generated_initializer_."));
                    Statement?.AddSourcePart(new Span(capture.GetIdentifierName()));
                    break;
                }

                case ILocalSymbol local:
                    Statement?.AddSourcePart(new Span(local.Name));
                    break;
            }
        }

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            Assignment assignment;

            using (var leftScope = SyntaxCaptureScope<Expression>.Create(this, WellKnownSyntax.AssignmentExpressionSyntax, new Expression()))
            {
                Visit(node.Left);

                using (var rightScope = SyntaxCaptureScope<Expression>.Create(this, WellKnownSyntax.AssignmentExpressionSyntax, new Expression()))
                {
                    Visit(node.Right);

                    assignment = new Assignment(node.OperatorToken.ValueText, leftScope.Statement, rightScope.Statement);
                }
            }

            Statement?.AddSourcePart(assignment);
        }

        public override void VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            var value = _context.SemanticModel.GetConstantValue(node);
            if (value.HasValue)
                Statement?.AddSourcePart(new Span(value!.Value!.ToString()!));
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            var info = _context.SemanticModel.GetSymbolInfo(node.Expression);
            if (info.Symbol is IMethodSymbol symbol)
            {
                var capture = new MethodSymbolCapture(symbol, _context.SemanticModel);
                if (capture.HasAttribute<CompilerAnnotatedAttribute>())
                    return;

                var function = new FunctionCall(capture.GetIdentifierName());
                using (SyntaxCaptureScope<FunctionCall>.Create(this, WellKnownSyntax.InvocationExpressionSyntax, function))
                    Visit(node.ArgumentList);
                Statement?.AddSourcePart(function);
            }
        }

        public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            var s = _context.SemanticModel.GetSymbolInfo(node);

            switch (s.Symbol)
            {
                case IMethodSymbol m:
                {
                    var capture = new MethodSymbolCapture(m, _context.SemanticModel);
                    Statement?.OfType<FunctionCall>()?.AddIdentifier(capture.GetIdentifierName());

                    Visit(node.Expression);
                    break;
                }

                case IPropertySymbol p:
                {
                    var memberAccess = new MemberAccess();
                    using (SyntaxCaptureScope<MemberAccess>.Create(this, WellKnownSyntax.MemberAccessExpressionSyntax, memberAccess))
                        Visit(node.Expression);

                    var capture = new PropertySymbolCapture(p, _context.SemanticModel);
                    memberAccess.AddSourcePart(new Span(capture.GetIdentifierName()));
                    Statement?.AddSourcePart(memberAccess);

                    break;
                }
            }
        }

        public override void VisitInitializerExpression(InitializerExpressionSyntax node)
        {
            foreach (var expression in node.Expressions)
            {
                var statement = new Statement();
                using (SyntaxCaptureScope<Statement>.Create(this, WellKnownSyntax.InitializerExpressionSyntax, statement))
                    Visit(expression);

                Statement?.AddSourcePart(statement);
            }
        }

        public override void VisitImplicitObjectCreationExpression(ImplicitObjectCreationExpressionSyntax node)
        {
            var t = _context.SemanticModel.GetTypeInfo(node);
            var capture = TypeDeclarationCapture.Capture(t, _context.SemanticModel);
            VisitObjectCreation(node, capture);
        }

        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            var capture = TypeDeclarationCapture.Capture(node.Type, _context.SemanticModel);
            VisitObjectCreation(node, capture);
        }

        public override void VisitBlock(BlockSyntax node)
        {
            if (CurrentCapturing == WellKnownSyntax.MethodDeclarationSyntax)
                using (var scope = SyntaxCaptureScope<Block>.Create(this, WellKnownSyntax.BlockSyntax, new Block()))
                {
                    foreach (var statement in node.Statements)
                        Visit(statement);

                    _context.SourceContext.OfType<ShaderLabHLSLSourceContext>()?.FunctionDeclaration?.AddSourcePart(scope.Statement);
                }
        }

        public override void VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            var capture = TypeDeclarationCapture.Capture(node.Type, _context.SemanticModel);

            foreach (var variable in node.Variables)
            {
                if (variable.ArgumentList != null)
                    _context.Errors.Add(new DefaultError(variable.ArgumentList, "SharpX.ShaderLab does not currently supports bracket argument list yet"));

                var statement = new Statement();
                using (var scope = SyntaxCaptureScope<VariableDeclaration>.Create(this, WellKnownSyntax.VariableDeclarationSyntax, new VariableDeclaration(capture.GetActualName(), variable.Identifier.ValueText)))
                {
                    Visit(variable.Initializer);
                    statement.AddSourcePart(scope.Statement);
                }

                Statement?.AddSourcePart(statement);
            }
        }

        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
            var declaration = _context.SourceContext.OfType<ShaderLabHLSLSourceContext>()?.FunctionDeclaration;
            if (declaration == null)
                return;

            var statement = new Statement();
            using (var scope = SyntaxCaptureScope<ReturnStatement>.Create(this, WellKnownSyntax.ReturnStatementSyntax, new ReturnStatement()))
            {
                Visit(node.Expression);
                statement.AddSourcePart(scope.Statement);
            }

            Statement?.AddSourcePart(statement);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            VisitTypeDeclaration(node);
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            VisitTypeDeclaration(node);
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            VisitTypeDeclaration(node);
        }

        public override void VisitRecordDeclaration(RecordDeclarationSyntax node)
        {
            VisitTypeDeclaration(node);
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
                    using (var scope = SyntaxCaptureScope<Block>.Create(this, WellKnownSyntax.BlockSyntax, new Block()))
                    {
                        Visit(node.ExpressionBody);

                        context.FunctionDeclaration.AddSourcePart(scope.Statement);
                    }
            }

            context.CloseFunction();
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

        public override void VisitParameter(ParameterSyntax node)
        {
            if (node.Type == null)
            {
                _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler requires the type to be specified for parameters"));
                return;
            }

            var capture = TypeDeclarationCapture.Capture(node.Type, _context.SemanticModel);
            var context = _context.SourceContext.OfType<ShaderLabHLSLSourceContext>();

            if (CurrentCapturing == WellKnownSyntax.MethodDeclarationSyntax)
            {
                ProcessInclude(capture);

                if (capture.IsArray())
                {
                    // NOTE: Is it possible to have a function that accepts an array with Semantics?
                    var attr = node.GetAttribute<InputPrimitiveAttribute>(_context.SemanticModel);
                    if (attr == null)
                    {
                        var name = $"{node.Identifier.ValueText}[]";
                        context?.FunctionDeclaration?.AddArgument(capture.GetActualName(), name);
                    }
                    else
                    {
                        var name = $"{node.Identifier.ValueText}[{attr.Primitives.GetArrayElement()}]";
                        context?.FunctionDeclaration?.AddAttributedArgument(attr.Primitives.ToString().ToLowerInvariant(), capture.GetActualName(), name);
                    }
                }
                else if (node.HasAttribute<SemanticAttribute>(_context.SemanticModel))
                {
                    var attr = node.GetAttribute<SemanticAttribute>(_context.SemanticModel);
                    if (!attr!.IsValidSemantics())
                        _context.Warnings.Add(new DefaultError(node, "The format of the string specified for Semantics is not correct"));

                    context?.FunctionDeclaration?.AddArgumentWithSemantics(capture.GetActualName(), node.Identifier.ValueText, attr.Semantic);
                }
                else
                {
                    context?.FunctionDeclaration!.AddArgument(capture.GetActualName(), node.Identifier.ValueText);
                }
            }
        }

        #region Common Utilities

        private void ProcessInclude(TypeDeclarationCapture capture)
        {
            if (capture.HasAttribute<IncludeAttribute>())
            {
                var attr = capture.GetAttribute<IncludeAttribute>();
                _context.SourceContext.OfType<ShaderLabHLSLSourceContext>()?.AddGlobalInclude(attr!.FileName);
            }
        }

        private void VisitObjectCreation(BaseObjectCreationExpressionSyntax node, TypeDeclarationCapture capture)
        {
            if (node.Initializer == null)
            {
                if (capture.HasAttribute<ExternalAttribute>() && capture.GetConstructors().Any(w => w.HasAttribute<ConstructorAttribute>()))
                {
                    // allowed constructors
                    var constructor = new Constructor(capture.GetActualName());
                    using (SyntaxCaptureScope<Constructor>.Create(this, WellKnownSyntax.ObjectCreationExpressionSyntax, constructor))
                        Visit(node.ArgumentList);

                    Statement?.AddSourcePart(constructor);
                }
                else
                {
                    _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab only allows externally defined constructors. Use Initializer instead of constructor to initialize self-defined structures"));
                }
            }
            else
            {
                if (node.ArgumentList?.Arguments.Count > 0)
                    _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab does not allow you to use Initializer and constructor at the same time"));

                if (CapturingStack.Contains(WellKnownSyntax.ObjectCreationExpressionSyntax))
                    _context.Errors.Add(new DefaultError(node, "Initializer cannot be nested in SharpX.ShaderLab at this time"));

                var context = _context.SourceContext.OfType<ShaderLabHLSLSourceContext>();

                // create a internal function for initialize user-defined struct
                var constructor = Naming.GetSafeName($"internal_constructor_{capture.GetActualName()}");
                context?.OpenFunction(constructor, capture.GetActualName());

                var arguments = new List<ISymbol>();
                foreach (var symbol in _context.SemanticModel.LookupSymbols(node.SpanStart).Where(w => w is ILocalSymbol or IParameterSymbol).ToList())
                    if (symbol.DeclaringSyntaxReferences.All(w => !node.InParent(w.GetSyntax())))
                    {
                        var s = symbol switch
                        {
                            ILocalSymbol l => l.Type,
                            IParameterSymbol p => p.Type,
                            _ => throw new ArgumentOutOfRangeException()
                        };

                        var c = TypeDeclarationCapture.Capture(s, _context.SemanticModel);
                        context?.FunctionDeclaration?.AddArgument(c.GetActualName(), symbol.Name);
                        arguments.Add(symbol);
                    }

                using (var scope = SyntaxCaptureScope<Block>.Create(this, WellKnownSyntax.ObjectCreationExpressionSyntax, new Block()))
                {
                    scope.Statement.AddSourcePart(new VariableDeclaration(capture.GetActualName(), "_auto_generated_initializer_", new Span($"({capture.GetActualName()}) 0")).IntoStatement());
                    Visit(node.Initializer);
                    scope.Statement.AddSourcePart(new ReturnStatement(new Span("_auto_generated_initializer_")).IntoStatement());

                    context?.FunctionDeclaration?.AddSourcePart(scope.Statement);
                }

                context?.CloseFunction();

                var call = new FunctionCall(constructor);
                foreach (var argument in arguments)
                    call.AddSourcePart(new Span(argument.Name));
                Statement?.AddSourcePart(call);

                context?.AddDependencyTree(constructor);
            }
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

        #endregion

        #region Unsupported Syntaxes

        public override void VisitArrayType(ArrayTypeSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support array types"));
        }

        public override void VisitPointerType(PointerTypeSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support pointer types"));
        }

        public override void VisitFunctionPointerType(FunctionPointerTypeSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support function pointer features"));
        }

        public override void VisitFunctionPointerParameterList(FunctionPointerParameterListSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support function pointer features"));
        }

        public override void VisitFunctionPointerCallingConvention(FunctionPointerCallingConventionSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support function pointer features"));
        }

        public override void VisitFunctionPointerUnmanagedCallingConventionList(FunctionPointerUnmanagedCallingConventionListSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support function pointer features"));
        }

        public override void VisitFunctionPointerUnmanagedCallingConvention(FunctionPointerUnmanagedCallingConventionSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support function pointer features"));
        }

        public override void VisitNullableType(NullableTypeSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support nullable types"));
        }

        public override void VisitRefType(RefTypeSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support ref types. This feature is handled by the backend, not by the on SharpX"));
        }

        public override void VisitAwaitExpression(AwaitExpressionSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support async-await expressions"));
        }

        public override void VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support conditional access expressions"));
        }

        public override void VisitMakeRefExpression(MakeRefExpressionSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support ref types. This feature is handled by the backend, not by the on SharpX"));
        }

        public override void VisitRefTypeExpression(RefTypeExpressionSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support ref types. This feature is handled by the backend, not by the on SharpX"));
        }

        public override void VisitRefValueExpression(RefValueExpressionSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support ref types. This feature is handled by the backend, not by the on SharpX"));
        }

        public override void VisitCheckedExpression(CheckedExpressionSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support checked expressions"));
        }

        public override void VisitTypeOfExpression(TypeOfExpressionSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support typeof expressions"));
        }

        public override void VisitSizeOfExpression(SizeOfExpressionSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support sizeof expressions"));
        }

        public override void VisitRefExpression(RefExpressionSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support ref types. This feature is handled by the backend, not by the on SharpX"));
        }

        public override void VisitStackAllocArrayCreationExpression(StackAllocArrayCreationExpressionSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support stackalloc"));
        }

        public override void VisitImplicitStackAllocArrayCreationExpression(ImplicitStackAllocArrayCreationExpressionSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support stackalloc"));
        }

        public override void VisitQueryExpression(QueryExpressionSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support LINQ features"));
        }

        public override void VisitQueryBody(QueryBodySyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support LINQ features"));
        }

        public override void VisitFromClause(FromClauseSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support LINQ features"));
        }

        public override void VisitLetClause(LetClauseSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support LINQ features"));
        }

        public override void VisitJoinClause(JoinClauseSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support LINQ features"));
        }

        public override void VisitJoinIntoClause(JoinIntoClauseSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support LINQ features"));
        }

        public override void VisitWhereClause(WhereClauseSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support LINQ features"));
        }

        public override void VisitOrderByClause(OrderByClauseSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support LINQ features"));
        }

        public override void VisitOrdering(OrderingSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support LINQ features"));
        }

        public override void VisitSelectClause(SelectClauseSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support LINQ features"));
        }

        public override void VisitGroupClause(GroupClauseSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support LINQ features"));
        }

        public override void VisitQueryContinuation(QueryContinuationSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support LINQ features"));
        }

        public override void VisitThrowExpression(ThrowExpressionSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support throwing exceptions"));
        }

        public override void VisitGotoStatement(GotoStatementSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support goto: https://www.wikiwand.com/en/Spaghetti_code"));
        }

        public override void VisitThrowStatement(ThrowStatementSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support throwing exceptions"));
        }

        public override void VisitYieldStatement(YieldStatementSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support throwing exceptions"));
        }

        public override void VisitUsingStatement(UsingStatementSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support using statement"));
        }

        public override void VisitFixedStatement(FixedStatementSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support fixed statement"));
        }

        public override void VisitCheckedStatement(CheckedStatementSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support checked statement"));
        }

        public override void VisitUnsafeStatement(UnsafeStatementSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support unsafe statement"));
        }

        public override void VisitLockStatement(LockStatementSyntax node)
        {
            _context.Errors.Add(new DefaultError(node, "SharpX.ShaderLab Compiler does not support lock statement"));
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