using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;
using SharpX.Compiler.Extensions;
using SharpX.Compiler.Udon.Models.Captures;
using SharpX.Compiler.Udon.Models.Declarators;
using SharpX.Compiler.Udon.Models.Symbols;
using SharpX.Library.Udon;
using SharpX.Library.Udon.Attributes;
using SharpX.Library.Udon.Enums;

using ISymbol = Microsoft.CodeAnalysis.ISymbol;

namespace SharpX.Compiler.Udon.Models
{
    internal class UdonCSharpSyntaxWalker : CSharpSyntaxWalker
    {
        private readonly Stack<string> _capturingMethodNameStack;
        private readonly Stack<Dictionary<ISymbol, VariableSymbol>> _capturingVariablesStack;
        private readonly ILanguageSyntaxWalkerContext _context;
        private int _currentCapturingParameterIndex;
        private INamedTypeSymbol? _currentClass;
        private UdonSymbolTable? _symbolTable;

        private string? CurrentCaptureMethodName => _capturingMethodNameStack.Count > 0 ? _capturingMethodNameStack.Peek() : null;

        private Dictionary<ISymbol, VariableSymbol> CurrentCaptureVariables => _capturingVariablesStack.Count > 0 ? _capturingVariablesStack.Peek() : new Dictionary<ISymbol, VariableSymbol>();

        public UdonCSharpSyntaxWalker(ILanguageSyntaxWalkerContext context) : base(SyntaxWalkerDepth.Token)
        {
            _context = context;
            _capturingMethodNameStack = new Stack<string>();
            _capturingVariablesStack = new Stack<Dictionary<ISymbol, VariableSymbol>>();
            _currentCapturingParameterIndex = 0;
        }

        public override void DefaultVisit(SyntaxNode node)
        {
            base.DefaultVisit(node);
        }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            base.VisitIdentifierName(node);
        }

        public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            var info = _context.SemanticModel.GetSymbolInfo(node.Expression);
            if (info.Symbol == null)
                return;

            var symbol = info.Symbol;
            var context = _context.SourceContext.OfType<UdonSourceContext>()!.UasmBuilder.CurrentMethodAssemblyBuilder!;

            if (CurrentCaptureVariables.ContainsKey(symbol))
            {
                if (symbol is IFieldSymbol)
                    // is capturing fields
                    context.AddPush(_symbolTable.GetNamedSymbol(symbol.Name)!);
                else if (symbol is IParameterSymbol)
                    context.AddPush(CurrentCaptureVariables.First(w => w.Key.Equals(symbol, SymbolEqualityComparer.Default)).Value);
                else
                    Debug.WriteLine("Who are you?");
            }

            if (symbol.ContainingType?.BaseType?.Equals(_context.SemanticModel.Compilation.GetTypeByMetadataName(typeof(SharpXUdonBehaviour).FullName!), SymbolEqualityComparer.Default) == true)
            {
                if (symbol.ContainingType.BaseType.Equals(_currentClass, SymbolEqualityComparer.Default))
                    Debug.WriteLine("Maybe this types");
                else
                    Debug.WriteLine("Maybe user-defined types");
            }
        }

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            Visit(node.Right);

            Visit(node.Left);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var declarator = TypeDeclarationDeclarator.Create(node, _context.SemanticModel);

            // compiled as Udon
            if (declarator.IsInherited<SharpXUdonBehaviour>())
            {
                _context.CreateOrGetContext(declarator.GetFullyQualifiedName());
                _symbolTable = new UdonSymbolTable();
                _currentClass = _context.SemanticModel.GetDeclaredSymbol(node)!;
                _capturingVariablesStack.Push(new Dictionary<ISymbol, VariableSymbol>());

                foreach (var member in node.Members)
                    Visit(member);

                foreach (var symbol in _symbolTable.VariableSymbols)
                    _context.SourceContext.OfType<UdonSourceContext>()!.UasmBuilder.AddVariableSymbol(symbol.Name, symbol.Type, symbol.IsExport, symbol.SyncMode, symbol.InitialValue);
                foreach (var symbol in _symbolTable.NamedAddressSymbols)
                    _context.SourceContext.OfType<UdonSourceContext>()!.UasmBuilder.AddVariableSymbol(symbol.Name, "SystemUInt32", false, null, "null");

                _context.CloseContext();
                _symbolTable.Dispose();
                _capturingVariablesStack.Pop();
            }
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            var context = _context.SourceContext.OfType<UdonSourceContext>()!;
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

                var variableSymbol = new VariableSymbol(variable.Identifier.ValueText, capture.GetUdonName(), export, sync, "null");
                _symbolTable!.AddNamedSymbol(variableSymbol);
                CurrentCaptureVariables.Add(symbol!, variableSymbol);
            }
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var name = node.Identifier.ValueText;
            if (name == "Awake")
                _context.Errors.Add(new VisualStudioCatchError(node, "Awake event does not supported by Udon, use Start event instead", ErrorConstants.NotSupportedAwakeEvent));

            _capturingMethodNameStack.Push(UdonNodeResolver.Instance.RemappedBuiltinEvent(name));
            _currentCapturingParameterIndex = 0;
            _capturingVariablesStack.Push(new Dictionary<ISymbol, VariableSymbol>(CurrentCaptureVariables));

            var returnType = TypeDeclarationCapture.Capture(node.ReturnType, _context.SemanticModel);
            var arguments = node.ParameterList.Parameters.Select(w => TypeDeclarationCapture.Capture(w.Type!, _context.SemanticModel)).Select(w => w.GetUdonName()).ToArray();
            var isExport = UdonNodeResolver.Instance.IsBuiltinEvent(name) || node.Modifiers.Any(SyntaxKind.PublicKeyword);

            var context = _context.SourceContext.OfType<UdonSourceContext>()!;

            if (!returnType.HasValidType())
                _context.Errors.Add(new VisualStudioCatchError(node, $"The return type {returnType.GetActualName()} does not supported by Udon", ErrorConstants.NotSupportedUdonType));

            context.UasmBuilder.StartMethod(UdonNodeResolver.Instance.RemappedBuiltinEvent(name), returnType.GetUdonName(), arguments, isExport);

            context.UasmBuilder.CurrentMethodAssemblyBuilder!.AddPush(_symbolTable!.GetConstantSymbol("", 0x00000000));

            foreach (var parameter in node.ParameterList.Parameters)
            {
                Visit(parameter);
                _currentCapturingParameterIndex++;

                if (parameter.Modifiers.Any(SyntaxKind.InKeyword))
                    _context.Errors.Add(new VisualStudioCatchError(parameter, "SharpX.Udon does not support `in` parameters on user-defined types", ErrorConstants.NotSupportedInParameter));
                if (parameter.Modifiers.Any(SyntaxKind.OutKeyword))
                    _context.Errors.Add(new VisualStudioCatchError(parameter, "SharpX.Udon does not support `out` parameters on user-defined types", ErrorConstants.NotSupportedOutParameter));
                if (parameter.Modifiers.Any(SyntaxKind.RefKeyword))
                    _context.Errors.Add(new VisualStudioCatchError(parameter, "SharpX.Udon does not support `ref` parameters on user-defined types", ErrorConstants.NotSupportedOutParameter));
            }


            if (node.Body != null)
                Visit(node.Body);
            else if (node.ExpressionBody != null)
                Visit(node.ExpressionBody);

            context.UasmBuilder.CurrentMethodAssemblyBuilder!.AddPush(_symbolTable!.GetConstantSymbol("returnTarget", 0xFFFFFFFF));
            context.UasmBuilder.CurrentMethodAssemblyBuilder!.AddCopy();
            context.UasmBuilder.CurrentMethodAssemblyBuilder!.AddJumpIndirect(_symbolTable!.GetConstantSymbol("returnTarget", 0xFFFFFFFF));

            context.UasmBuilder.CloseMethod();

            _capturingMethodNameStack.Pop();
            _capturingVariablesStack.Pop();
        }

        public override void VisitParameter(ParameterSyntax node)
        {
            var symbol = _context.SemanticModel.GetDeclaredSymbol(node);
            var parameter = UdonNodeResolver.Instance.GetMethodCustomArgs(CurrentCaptureMethodName!)?[_currentCapturingParameterIndex];
            if (parameter != null && parameter.Item2 != symbol!.Name)
            {
                // copy arguments to user-named variables
                var builder = _context.SourceContext.OfType<UdonSourceContext>()!.UasmBuilder.CurrentMethodAssemblyBuilder!;
                builder.AddPush(new VariableSymbol(parameter.Item2, "", false, null, "null"));

                var t = TypeDeclarationCapture.Capture(symbol, _context.SemanticModel);
                var variable = _symbolTable!.CreateNamedSymbol(node.Identifier.ValueText, t.GetUdonName());

                builder.AddPush(variable);
                builder.AddCopy();

                CurrentCaptureVariables.Add(symbol, variable);
            }
            else
            {
                var t = TypeDeclarationCapture.Capture(symbol!, _context.SemanticModel);
                var variable = _symbolTable!.CreateNamedSymbol(node.Identifier.ValueText, t.GetUdonName());
                CurrentCaptureVariables.Add(symbol!, variable);
            }
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node) { }

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

        #endregion
    }
}