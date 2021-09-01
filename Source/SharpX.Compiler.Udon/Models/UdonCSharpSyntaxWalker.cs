using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Enums;
using SharpX.Compiler.Composition.Interfaces;
using SharpX.Compiler.Extensions;
using SharpX.Compiler.Udon.Enums;
using SharpX.Compiler.Udon.Models.Captures;
using SharpX.Compiler.Udon.Models.Declarators;
using SharpX.Compiler.Udon.Models.Symbols;
using SharpX.Library.Udon;
using SharpX.Library.Udon.Attributes;
using SharpX.Library.Udon.Enums;

namespace SharpX.Compiler.Udon.Models
{
    internal class UdonCSharpSyntaxWalker : CSharpSyntaxWalker
    {
        private readonly ILanguageSyntaxWalkerContext _context;
        private readonly SafeStack<string> _jumpStack;
        private INamedTypeSymbol? _currentClass;

        private UdonSymbol? CurrentDestinationSymbol => DestinationSymbolStack.SafePeek();

        private WellKnownSyntax CurrentCapturingStack => CapturingStack.SafePeek(WellKnownSyntax.CompilationUnitSyntax);

        private string? CurrentCaptureMethodName => MethodCapturingStack.SafePeek();

        private SafeStack<UdonSymbol>? CurrentCapturingExpressions => ExpressionCapturingStack.SafePeek();

        private MethodUasmBuilder? MethodAssemblyBuilder => _context.SourceContext.OfType<UdonSourceContext>()?.UasmBuilder.CurrentMethodAssemblyBuilder;

        public UdonCSharpSyntaxWalker(ILanguageSyntaxWalkerContext context) : base(SyntaxWalkerDepth.Token)
        {
            _context = context;
            _jumpStack = new SafeStack<string>();

            CapturingStack = new SafeStack<WellKnownSyntax>();
            DestinationSymbolStack = new SafeStack<UdonSymbol?>();
            ExpressionCapturingStack = new SafeStack<SafeStack<UdonSymbol>>();
            IsGetterContext = new SafeStack<bool?>();
            MethodCapturingStack = new SafeStack<string?>();
        }

        public override void DefaultVisit(SyntaxNode node)
        {
            base.DefaultVisit(node);
        }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            if (SymbolTable == null)
                return;

            var info = _context.SemanticModel.GetSymbolInfo(node);
            if (info.Symbol != null && SymbolTable.GetAssociatedSymbol(info.Symbol) != null)
            {
                var symbol = SymbolTable.GetAssociatedSymbol(info.Symbol)!;
                CurrentCapturingExpressions?.Push(symbol);
                return;
            }

            switch (info.Symbol)
            {
                case IPropertySymbol property:
                {
                    if (!UdonNodeResolver.Instance.IsValidPropertyAccessor(property, _context.SemanticModel, IsGetterContext.SafePeek(false).Value))
                        _context.Errors.Add(new VisualStudioCatchError(node, $"The method {property.Name} does not supported by Udon", ErrorConstants.NotSupportedUdonMethod));


                    var t = UdonNodeResolver.Instance.GetUdonTypeName(property.Type, _context.SemanticModel);
                    var destinationSymbol = CurrentDestinationSymbol ?? SymbolTable.CreateUnnamedThisSymbol(t,$"this.{property.Name}");

                    CurrentCapturingExpressions?.Push(destinationSymbol);
                    break;
                }
            }
        }

        public override void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            UdonSymbol destinationSymbol;

            using (var expression = new ExpressionCaptureScope(this))
            {
                var operatorMethod = _context.SemanticModel.GetSymbolInfo(node);
                if (operatorMethod.Symbol is not IMethodSymbol symbol)
                {
                    _context.Errors.Add(new VisualStudioCatchError(node, "Failed to capture symbols from semantic model", ErrorConstants.FailedToCaptureSymbols));
                    return;
                }

                var @operator = UdonNodeResolver.Instance.RemappedToBuiltinOperator(symbol, _context.SemanticModel, node.OperatorToken.Kind());
                if (!UdonNodeResolver.Instance.IsValidMethod(@operator))
                    _context.Errors.Add(new VisualStudioCatchError(node, $"The method {symbol.Name} does not supported by Udon", ErrorConstants.NotSupportedUdonMethod));

                Visit(node.Operand);

                var t = UdonNodeResolver.Instance.GetUdonTypeName(symbol.ReturnType, _context.SemanticModel);
                destinationSymbol = CurrentDestinationSymbol ?? SymbolTable!.CreateUnnamedSymbol(t, UdonSymbolDeclarations.Private);

                MethodAssemblyBuilder?.AddPush(expression.CapturingExpressions.Pop());
                MethodAssemblyBuilder?.AddPush(destinationSymbol);
                MethodAssemblyBuilder?.AddExtern(@operator);
            }

            CurrentCapturingExpressions?.Push(destinationSymbol);
        }

        public override void VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
            Visit(node.Operand);
        }

        public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            UdonSymbol? destinationSymbol = null;

            using (var capture = new ExpressionCaptureScope(this, true, CurrentDestinationSymbol))
            {
                Visit(node.Expression);

                var info = _context.SemanticModel.GetSymbolInfo(node);
                if (info.Symbol is not (IFieldSymbol or IMethodSymbol or IPropertySymbol))
                {
                    _context.Errors.Add(new VisualStudioCatchError(node, "Failed to capture symbols by semantic model", ErrorConstants.FailedToCaptureSymbols));
                    return;
                }

                var t = info.Symbol is IFieldSymbol f1 ? f1.Type : info.Symbol is IMethodSymbol m1 ? m1.ReturnType : ((IPropertySymbol)info.Symbol).Type;
                var n = UdonNodeResolver.Instance.GetUdonTypeName(t, _context.SemanticModel);
                var s = info.Symbol switch
                {
                    IMethodSymbol m => UdonNodeResolver.Instance.GetUdonMethodName(m, _context.SemanticModel),
                    IPropertySymbol p => UdonNodeResolver.Instance.GetUdonPropertyAccessorName(p, _context.SemanticModel, true),
                    IFieldSymbol f => UdonNodeResolver.Instance.GetUdonPropertyAccessorName(f, _context.SemanticModel, true),
                    _ => throw new ArgumentOutOfRangeException()
                };

                if (!UdonNodeResolver.Instance.IsValidMethod(s))
                    _context.Errors.Add(new VisualStudioCatchError(node, $"The method or field {info.Symbol.Name} does not supported by Udon", ErrorConstants.NotSupportedUdonMethod));

                if (capture.CapturingExpressions.SafePeek() != null)
                    MethodAssemblyBuilder?.AddPush(capture.CapturingExpressions.Pop());

                if (t.SpecialType != SpecialType.System_Void)
                {
                    destinationSymbol = CurrentDestinationSymbol ?? SymbolTable!.CreateUnnamedSymbol(n, UdonSymbolDeclarations.Private);
                    MethodAssemblyBuilder?.AddPush(destinationSymbol);
                }

                MethodAssemblyBuilder?.AddExtern(s);
            }

            if (destinationSymbol != null)
                CurrentCapturingExpressions?.Push(destinationSymbol);
        }

        public override void VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            var info = _context.SemanticModel.GetSymbolInfo(node);
            if (info.Symbol is not IMethodSymbol methodCall)
            {
                _context.Errors.Add(new VisualStudioCatchError(node, "Failed to capture symbols by semantic model", ErrorConstants.FailedToCaptureSymbols));
                return;
            }

            if (!UdonNodeResolver.Instance.IsValidMethod(methodCall, _context.SemanticModel))
                _context.Errors.Add(new VisualStudioCatchError(node, $"The method {methodCall.Name} is not supported by Udon", ErrorConstants.NotSupportedUdonMethod));

            UdonSymbol? destinationSymbol = null;

            using (var expression = new ExpressionCaptureScope(this, true, CurrentDestinationSymbol))
            {
                Visit(node.Right);
                Visit(node.Left);

                while (expression.CapturingExpressions.SafePeek() != null)
                {
                    destinationSymbol = expression.CapturingExpressions.Pop();
                    MethodAssemblyBuilder?.AddPush(destinationSymbol);
                }

                if (expression.DestinationSymbol != null)
                {
                    destinationSymbol = expression.DestinationSymbol;
                    MethodAssemblyBuilder?.AddPush(expression.DestinationSymbol);
                }

                var name = UdonNodeResolver.Instance.GetUdonMethodName(methodCall, _context.SemanticModel);
                MethodAssemblyBuilder?.AddExtern(name);
            }

            if (destinationSymbol != null)
                CurrentCapturingExpressions?.Push(destinationSymbol);
        }

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            using (var left = new ExpressionCaptureScope(this))
            {
                Visit(node.Left);

                var destination = left.CapturingExpressions.Pop();

                using (var right = new ExpressionCaptureScope(this, true, destination))
                {
                    Visit(node.Right);

                    var source = right.CapturingExpressions.Pop();

                    if (node.OperatorToken.Kind() is SyntaxKind.SimpleAssignmentExpression or SyntaxKind.EqualsToken && source != destination)
                    {
                        MethodAssemblyBuilder?.AddPush(source);
                        MethodAssemblyBuilder?.AddPush(destination);
                        MethodAssemblyBuilder?.AddCopy();

                    }
                }
            }
        }

        public override void VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            var value = _context.SemanticModel.GetConstantValue(node);

            var symbol = node.Kind() switch
            {
                SyntaxKind.NumericLiteralExpression => SymbolTable?.CreateOrGetUnnamedConstantSymbol(UdonNodeResolver.Instance.GetUdonTypeName(node.Token.Value!.GetType(), _context.SemanticModel), value.Value!),
                SyntaxKind.StringLiteralExpression => SymbolTable?.CreateOrGetUnnamedConstantSymbol("SystemString", value.Value!),
                SyntaxKind.CharacterLiteralToken => SymbolTable?.CreateOrGetUnnamedConstantSymbol("SystemChar", value.Value!),
                SyntaxKind.TrueLiteralExpression => SymbolTable?.CreateOrGetUnnamedConstantSymbol("SystemBoolean", true),
                SyntaxKind.FalseLiteralExpression => SymbolTable?.CreateOrGetUnnamedConstantSymbol("SystemBoolean", false),
                SyntaxKind.NullLiteralExpression => SymbolTable?.CreateOrGetUnnamedConstantSymbol("SystemObject", null),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (symbol == null)
                return;

            CurrentCapturingExpressions?.Push(symbol);
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            UdonSymbol? candidateDestinationSymbol = null;

            using (var invocation = new ExpressionCaptureScope(this, IsGetterContext.SafePeek(false).Value, CurrentDestinationSymbol))
            {
                var args = new List<UdonSymbol>();
                foreach (var argument in node.ArgumentList.Arguments)
                {
                    if (argument.RefKindKeyword != default)
                        _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon does not support ref or out keyword in arguments", ErrorConstants.NotSupportedRefParameter));
                    if (argument.NameColon != null)
                        _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon does not support named parameter arguments", ErrorConstants.NotSupportedNamedParameter));

                    var info = _context.SemanticModel.GetSymbolInfo(node.Expression);
                    if (info.Symbol != null && SymbolTable!.GetAssociatedSymbol(info.Symbol) != null)
                    {
                        var symbol = SymbolTable!.GetAssociatedSymbol(info.Symbol)!;
                        args.Add(symbol);
                        continue;
                    }

                    using (var capture = new ExpressionCaptureScope(this, true))
                    {
                        Visit(argument.Expression);

                        if (capture.CapturingExpressions.SafePeek() != null)
                            args.Add(capture.CapturingExpressions.Pop());
                    }
                }

                Visit(node.Expression);

                foreach (var symbol in args)
                   MethodAssemblyBuilder?.AddPushBeforeCurrent(symbol);


                candidateDestinationSymbol = invocation.CapturingExpressions.SafePeek();
            }

            if (CurrentDestinationSymbol == null && candidateDestinationSymbol != null)
                CurrentCapturingExpressions?.Push(candidateDestinationSymbol);
        }

        public override void VisitBlock(BlockSyntax node)
        {
            using (new UdonSymbolEnclosure(this))
            {
                MethodAssemblyBuilder?.AddComment("{");

                foreach (var statement in node.Statements)
                    Visit(statement);

                MethodAssemblyBuilder?.AddComment("}");
            }
        }

        public override void VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            var t = TypeDeclarationCapture.Capture(node.Type, _context.SemanticModel);
            if (!t.HasValidType())
                _context.Errors.Add(new VisualStudioCatchError(node, $"The type {t.GetActualName()} does not supported by Udon", ErrorConstants.NotSupportedUdonType));

            foreach (var variable in node.Variables)
            {
                var info = _context.SemanticModel.GetSymbolInfo(variable);
                if (info.Symbol == null)
                {
                    _context.Errors.Add(new VisualStudioCatchError(variable, "Failed to capture semantic model symbols", ErrorConstants.FailedToCaptureSymbols));
                    continue;
                }

                var symbol = info.Symbol;
                var namedSymbol = SymbolTable!.CreateNamedSymbol(t.GetUdonName(), variable.Identifier.ValueText);
                SymbolTable!.AssociateWithSymbol(namedSymbol, symbol);
            }
        }

        public override void VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            MethodAssemblyBuilder?.AddComment(node.ToString());

            base.VisitExpressionStatement(node);
        }

        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
            MethodAssemblyBuilder?.AddComment(node.ToString());

            if (node.Expression == null)
            {
                var returnSymbol = SymbolTable!.CreateOrGetNamedConstantSymbol("SystemUInt32", "returnTarget", 0xFFFFFFFF);
                MethodAssemblyBuilder?.AddPush(returnSymbol);
                MethodAssemblyBuilder?.AddCopy();
                MethodAssemblyBuilder?.AddJumpIndirect(returnSymbol);
                return;
            }

            using (var returnCapture = new ExpressionCaptureScope(this))
            {
                var t = _context.SemanticModel.GetTypeInfo(node.Expression);
                var u = UdonNodeResolver.Instance.GetUdonTypeName(t.Type, _context.SemanticModel);
                var symbol = SymbolTable!.CreateNamedSymbol(u, "returnValue");

                returnCapture.CapturingExpressions.Push(symbol);

                Visit(node.Expression);

                if (CurrentCaptureMethodName == "_onOwnershipRequest")
                {
                    const string n = "__returnValue";
                    var returnSymbol = new UdonSymbol("SystemObject", n, n, UdonSymbolDeclarations.Public, null, "null");
                    SymbolTable.AddNewSymbol(returnSymbol);
                    MethodAssemblyBuilder?.AddPush(symbol);
                    MethodAssemblyBuilder?.AddPush(returnSymbol);
                    MethodAssemblyBuilder?.AddCopy();
                }
            }
        }

        public override void VisitForStatement(ForStatementSyntax node)
        {
            base.VisitForStatement(node);
        }

        public override void VisitForEachStatement(ForEachStatementSyntax node)
        {
            // foreach (var el of array) { } -> for (var i = 0; i < array.Length; i++) { var el = array[i]; } 
            base.VisitForEachStatement(node);
        }

        public override void VisitIfStatement(IfStatementSyntax node)
        {
            MethodAssemblyBuilder?.AddComment($"if ({node.Condition}) {{");

            using (var condition = new ExpressionCaptureScope(this, true))
            {
                Visit(node.Condition);

                if (condition.CapturingExpressions.Count != 1)
                    _context.Errors.Add(new VisualStudioCatchError(node.Condition, "Internal Error", 9999));

                MethodAssemblyBuilder?.AddPush(condition.CapturingExpressions.Pop());
            }

            // using (SyntaxCaptureScope.Create(this, WellKnownSyntax.IfStatementSyntax, true))
            //    Visit(node.Condition);

            _jumpStack.Push(Guid.NewGuid().ToString());

            MethodAssemblyBuilder?.AddJumpIfFalseLabel(_jumpStack.SafePeek()!);

            using (new UdonSymbolEnclosure(this))
                Visit(node.Statement);

            MethodAssemblyBuilder?.AddComment("}");
            MethodAssemblyBuilder?.AddLabel(_jumpStack.Pop());

            if (node.Else != null)
                Visit(node.Else);
        }

        public override void VisitElseClause(ElseClauseSyntax node)
        {
            MethodAssemblyBuilder?.AddComment("else {");

            using (new UdonSymbolEnclosure(this))
                Visit(node.Statement);

            MethodAssemblyBuilder?.AddComment("}");
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var declarator = TypeDeclarationDeclarator.Create(node, _context.SemanticModel);

            // compiled as Udon
            if (declarator.IsInherited<SharpXUdonBehaviour>())
                try
                {
                    _context.CreateOrGetContext(declarator.GetFullyQualifiedName());
                    SymbolTable = new UdonSymbolTable();
                    _currentClass = _context.SemanticModel.GetDeclaredSymbol(node)!;

                    foreach (var member in node.Members)
                        Visit(member);

                    SymbolTable.ToFlatten();
                    foreach (var symbol in SymbolTable.ContextDefinedSymbols)
                        _context.SourceContext.OfType<UdonSourceContext>()!.UasmBuilder.AddVariableSymbol(symbol.UniqueName, symbol.Type, symbol.IsExport, symbol.SyncMode, symbol.ConstantValue);

                    _context.CloseContext();

                    // TODO: Write to Stack Heap
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            var capture = TypeDeclarationCapture.Capture(node.Declaration.Type, _context.SemanticModel);
            if (!capture.HasValidType())
            {
                _context.Errors.Add(new VisualStudioCatchError(node, $"The type {capture.GetActualName()} does not supported by Udon", ErrorConstants.NotSupportedUdonType));
                return;
            }

            var export = node.Modifiers.Any(SyntaxKind.PublicKeyword);

            foreach (var variable in node.Declaration.Variables)
            {
                var symbol = _context.SemanticModel.GetDeclaredSymbol(variable);

                // why loose???
                var sync = symbol.GetLooseAttribute<UdonSyncedAttribute>(_context.SemanticModel)?.SyncMode;
                if (symbol.HasLooseAttribute<UdonSyncedAttribute>(_context.SemanticModel))
                    sync = UdonSyncMode.None;


                var name = variable.Identifier.ValueText;
                var variableSymbol = new UdonSymbol(capture.GetUdonName(), name, name, UdonSymbolDeclarations.Public, null, "null", export, sync);
                SymbolTable!.AddNewSymbol(variableSymbol);
                SymbolTable!.AssociateWithSymbol(variableSymbol, symbol!);
            }
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var name = node.Identifier.ValueText;
            if (name == "Awake")
                _context.Errors.Add(new VisualStudioCatchError(node, "Awake event does not supported by Udon, use Start event instead", ErrorConstants.NotSupportedAwakeEvent));

            using (new MethodCapturingScope(this, node))
            {
                var returnType = TypeDeclarationCapture.Capture(node.ReturnType, _context.SemanticModel);
                var arguments = node.ParameterList.Parameters.Select(w => TypeDeclarationCapture.Capture(w.Type!, _context.SemanticModel)).Select(w => w.GetUdonName()).ToArray();
                var isExport = UdonNodeResolver.Instance.IsBuiltinEvent(name) || node.Modifiers.Any(SyntaxKind.PublicKeyword);

                var context = _context.SourceContext.OfType<UdonSourceContext>()!;

                if (!returnType.HasValidType())
                    _context.Errors.Add(new VisualStudioCatchError(node, $"The return type {returnType.GetActualName()} does not supported by Udon", ErrorConstants.NotSupportedUdonType));

                context.UasmBuilder.StartMethod(UdonNodeResolver.Instance.RemappedBuiltinEvent(name), returnType.GetUdonName(), arguments, isExport);

                context.UasmBuilder.CurrentMethodAssemblyBuilder!.AddComment($"{node.Modifiers} {node.ReturnType} {node.Identifier}{node.ParameterList}");
                context.UasmBuilder.CurrentMethodAssemblyBuilder!.AddPush(SymbolTable!.CreateOrGetUnnamedConstantSymbol("SystemUInt32", 0x00000000));

                foreach (var (parameter, index) in node.ParameterList.Parameters.Select((w, i) => (w, i)))
                {
                    if (parameter.Modifiers.Any(SyntaxKind.InKeyword))
                        _context.Errors.Add(new VisualStudioCatchError(parameter, "SharpX.Udon does not support `in` parameters on user-defined types", ErrorConstants.NotSupportedInParameter));
                    if (parameter.Modifiers.Any(SyntaxKind.OutKeyword))
                        _context.Errors.Add(new VisualStudioCatchError(parameter, "SharpX.Udon does not support `out` parameters on user-defined types", ErrorConstants.NotSupportedOutParameter));
                    if (parameter.Modifiers.Any(SyntaxKind.RefKeyword))
                        _context.Errors.Add(new VisualStudioCatchError(parameter, "SharpX.Udon does not support `ref` parameters on user-defined types", ErrorConstants.NotSupportedRefParameter));

                    var symbol = _context.SemanticModel.GetDeclaredSymbol(parameter);
                    var customArg = UdonNodeResolver.Instance.GetMethodCustomArgs(CurrentCaptureMethodName!)?[index];
                    if (customArg != null && customArg.Item2 != symbol!.Name)
                    {
                        // copy arguments to user-named variables
                        var param = new UdonSymbol(UdonNodeResolver.Instance.GetUdonTypeName(customArg.Item1, _context.SemanticModel), customArg.Item2, customArg.Item2, UdonSymbolDeclarations.Public, null, "null");
                        SymbolTable!.AddNewSymbol(param);
                        MethodAssemblyBuilder?.AddPush(param);

                        var t = TypeDeclarationCapture.Capture(symbol, _context.SemanticModel);
                        var variable = SymbolTable!.CreateNamedSymbol(t.GetUdonName(), parameter.Identifier.ValueText, UdonSymbolDeclarations.MethodParameter);

                        MethodAssemblyBuilder?.AddPush(variable);
                        MethodAssemblyBuilder?.AddCopy();

                        SymbolTable.AssociateWithSymbol(variable, symbol);
                    }
                    else
                    {
                        var t = TypeDeclarationCapture.Capture(symbol, _context.SemanticModel);
                        var variable = SymbolTable!.CreateNamedSymbol(t.GetUdonName(), parameter.Identifier.ValueText);
                        SymbolTable.AssociateWithSymbol(variable, symbol);
                    }
                }

                if (node.Body != null)
                {
                    Visit(node.Body);
                }
                else if (node.ExpressionBody != null)
                {
                    Visit(node.ExpressionBody);

                    var t = TypeDeclarationCapture.Capture(node.ReturnType, _context.SemanticModel);
                    if (!t.IsVoid()) { }
                }

                var returnSymbol = SymbolTable!.CreateOrGetNamedConstantSymbol("SystemUInt32", "returnTarget", 0xFFFFFFFF);
                context.UasmBuilder.CurrentMethodAssemblyBuilder!.AddPush(returnSymbol);
                context.UasmBuilder.CurrentMethodAssemblyBuilder!.AddCopy();
                context.UasmBuilder.CurrentMethodAssemblyBuilder!.AddJumpIndirect(returnSymbol);

                context.UasmBuilder.CloseMethod();
            }
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node) { }

        #region Capturing Stacks

        internal SafeStack<UdonSymbol?> DestinationSymbolStack { get; }

        internal SafeStack<SafeStack<UdonSymbol>> ExpressionCapturingStack { get; }

        internal UdonSymbolTable? SymbolTable { get; private set; }

        internal SafeStack<WellKnownSyntax> CapturingStack { get; }

        internal SafeStack<bool?> IsGetterContext { get; }

        internal SafeStack<string?> MethodCapturingStack { get; }

        #endregion

        #region UnSupported Syntaxes

        public override void VisitPointerType(PointerTypeSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support pointer types", ErrorConstants.NotSupportedPointerTypes));
        }

        public override void VisitFunctionPointerType(FunctionPointerTypeSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support function pointer features", ErrorConstants.NotSupportedFunctionPointers));
        }

        public override void VisitFunctionPointerParameterList(FunctionPointerParameterListSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support function pointer features", ErrorConstants.NotSupportedFunctionPointers));
        }

        public override void VisitFunctionPointerCallingConvention(FunctionPointerCallingConventionSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support function pointer features", ErrorConstants.NotSupportedFunctionPointers));
        }

        public override void VisitFunctionPointerUnmanagedCallingConventionList(FunctionPointerUnmanagedCallingConventionListSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support function pointer features", ErrorConstants.NotSupportedFunctionPointers));
        }

        public override void VisitFunctionPointerUnmanagedCallingConvention(FunctionPointerUnmanagedCallingConventionSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support function pointer features", ErrorConstants.NotSupportedFunctionPointers));
        }

        public override void VisitAwaitExpression(AwaitExpressionSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support async-await expressions", ErrorConstants.NotSupportedAsyncAwaitExpression));
        }

        public override void VisitMakeRefExpression(MakeRefExpressionSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support ref types", ErrorConstants.NotSupportedRefTypes));
        }

        public override void VisitRefTypeExpression(RefTypeExpressionSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support ref types", ErrorConstants.NotSupportedRefTypes));
        }

        public override void VisitRefValueExpression(RefValueExpressionSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support ref types", ErrorConstants.NotSupportedRefTypes));
        }

        public override void VisitCheckedExpression(CheckedExpressionSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support checked expressions", ErrorConstants.NotSupportedCheckedExpression));
        }

        public override void VisitSizeOfExpression(SizeOfExpressionSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support sizeof expressions", ErrorConstants.NotSupportedSizeofExpression));
        }

        public override void VisitRefExpression(RefExpressionSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support ref types. This feature is handled by the backend, not by the on SharpX", ErrorConstants.NotSupportedRefTypes));
        }

        public override void VisitStackAllocArrayCreationExpression(StackAllocArrayCreationExpressionSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support stackalloc", ErrorConstants.NotSupportedStackalloc));
        }

        public override void VisitImplicitStackAllocArrayCreationExpression(ImplicitStackAllocArrayCreationExpressionSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support stackalloc", ErrorConstants.NotSupportedStackalloc));
        }

        public override void VisitQueryExpression(QueryExpressionSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support LINQ features", ErrorConstants.NotSupportedLinqFeatures));
        }

        public override void VisitQueryBody(QueryBodySyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support LINQ features", ErrorConstants.NotSupportedLinqFeatures));
        }

        public override void VisitFromClause(FromClauseSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support LINQ features", ErrorConstants.NotSupportedLinqFeatures));
        }

        public override void VisitLetClause(LetClauseSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support LINQ features", ErrorConstants.NotSupportedLinqFeatures));
        }

        public override void VisitJoinClause(JoinClauseSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support LINQ features", ErrorConstants.NotSupportedLinqFeatures));
        }

        public override void VisitJoinIntoClause(JoinIntoClauseSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support LINQ features", ErrorConstants.NotSupportedLinqFeatures));
        }

        public override void VisitWhereClause(WhereClauseSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support LINQ features", ErrorConstants.NotSupportedLinqFeatures));
        }

        public override void VisitOrderByClause(OrderByClauseSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support LINQ features", ErrorConstants.NotSupportedLinqFeatures));
        }

        public override void VisitOrdering(OrderingSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support LINQ features", ErrorConstants.NotSupportedLinqFeatures));
        }

        public override void VisitSelectClause(SelectClauseSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support LINQ features", ErrorConstants.NotSupportedLinqFeatures));
        }

        public override void VisitGroupClause(GroupClauseSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support LINQ features", ErrorConstants.NotSupportedLinqFeatures));
        }

        public override void VisitQueryContinuation(QueryContinuationSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support LINQ features", ErrorConstants.NotSupportedLinqFeatures));
        }

        public override void VisitThrowExpression(ThrowExpressionSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support throwing exceptions", ErrorConstants.NotSupportedThrowingExceptions));
        }

        public override void VisitGotoStatement(GotoStatementSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support goto: https://www.wikiwand.com/en/Spaghetti_code", ErrorConstants.NotSupportedGoto));
        }

        public override void VisitThrowStatement(ThrowStatementSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support throwing exceptions", ErrorConstants.NotSupportedThrowingExceptions));
        }

        public override void VisitYieldStatement(YieldStatementSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support throwing exceptions", ErrorConstants.NotSupportedThrowingExceptions));
        }

        public override void VisitUsingStatement(UsingStatementSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support using statement", ErrorConstants.NotSupportedUsingStatement));
        }

        public override void VisitFixedStatement(FixedStatementSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support fixed statement", ErrorConstants.NotSupportedFixedStatement));
        }

        public override void VisitCheckedStatement(CheckedStatementSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support checked statement", ErrorConstants.NotSupportedCheckedStatement));
        }

        public override void VisitUnsafeStatement(UnsafeStatementSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support unsafe statement", ErrorConstants.NotSupportedUnsafeStatement));
        }

        public override void VisitLockStatement(LockStatementSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support lock statement", ErrorConstants.NotSupportedLockedStatement));
        }

        public override void VisitTryStatement(TryStatementSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support try-catch(-finally) statements", ErrorConstants.NotSupportedHandlingExceptions));
        }

        public override void VisitCatchClause(CatchClauseSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support try-catch(-finally) statements", ErrorConstants.NotSupportedHandlingExceptions));
        }

        public override void VisitCatchDeclaration(CatchDeclarationSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support try-catch(-finally) statements", ErrorConstants.NotSupportedHandlingExceptions));
        }

        public override void VisitCatchFilterClause(CatchFilterClauseSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support try-catch(-finally) statements", ErrorConstants.NotSupportedHandlingExceptions));
        }

        public override void VisitFinallyClause(FinallyClauseSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support try-catch(-finally) statements", ErrorConstants.NotSupportedHandlingExceptions));
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            _context.Errors.Add(new VisualStudioCatchError(node, "SharpX.Udon Compiler does not support user-defined constructors", ErrorConstants.NotSupportedUserDefinedConstructors));
        }

        #endregion
    }
}