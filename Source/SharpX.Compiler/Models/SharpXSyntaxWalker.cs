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
using SharpX.Compiler.Exceptions;
using SharpX.Compiler.Models.Plugin;

namespace SharpX.Compiler.Models
{
    internal class SharpXSyntaxWalker : CSharpSyntaxWalker
    {
        private readonly AssemblyContext _assembly;
        private readonly Compilation _compilation;
        private readonly List<string> _errors;
        private readonly SemanticModel _semanticModel;
        private readonly List<string> _warnings;
        private ISourceContext _context;

        public IReadOnlyCollection<string> Errors => _errors.AsReadOnly();

        public IReadOnlyCollection<string> Warnings => _warnings.AsReadOnly();

        public SharpXSyntaxWalker(Compilation compilation, SemanticModel semanticModel, AssemblyContext context) : base(SyntaxWalkerDepth.Token)
        {
            _compilation = compilation;
            _semanticModel = semanticModel;
            _assembly = context;
            _errors = new List<string>();
            _warnings = new List<string>();
            _context = _assembly.Default;
        }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            HookActions(WellKnownSyntax.IdentifierNameSyntax, node, () => base.VisitIdentifierName(node));
        }

        public override void VisitQualifiedName(QualifiedNameSyntax node)
        {
            HookActions(WellKnownSyntax.QualifiedNameSyntax, node, () => base.VisitQualifiedName(node));
        }

        public override void VisitGenericName(GenericNameSyntax node)
        {
            HookActions(WellKnownSyntax.GenericNameSyntax, node, () => base.VisitGenericName(node));
        }

        public override void VisitTypeArgumentList(TypeArgumentListSyntax node)
        {
            HookActions(WellKnownSyntax.TypeArgumentListSyntax, node, () => base.VisitTypeArgumentList(node));
        }

        public override void VisitAliasQualifiedName(AliasQualifiedNameSyntax node)
        {
            HookActions(WellKnownSyntax.AliasQualifiedNameSyntax, node, () => base.VisitAliasQualifiedName(node));
        }

        public override void VisitPredefinedType(PredefinedTypeSyntax node)
        {
            HookActions(WellKnownSyntax.PredefinedTypeSyntax, node, () => base.VisitPredefinedType(node));
        }

        public override void VisitArrayType(ArrayTypeSyntax node)
        {
            HookActions(WellKnownSyntax.ArrayTypeSyntax, node, () => base.VisitArrayType(node));
        }

        public override void VisitArrayRankSpecifier(ArrayRankSpecifierSyntax node)
        {
            HookActions(WellKnownSyntax.ArrayRankSpecifierSyntax, node, () => base.VisitArrayRankSpecifier(node));
        }

        public override void VisitPointerType(PointerTypeSyntax node)
        {
            HookActions(WellKnownSyntax.PointerTypeSyntax, node, () => base.VisitPointerType(node));
        }

        public override void VisitFunctionPointerType(FunctionPointerTypeSyntax node)
        {
            HookActions(WellKnownSyntax.FunctionPointerTypeSyntax, node, () => base.VisitFunctionPointerType(node));
        }

        public override void VisitFunctionPointerParameterList(FunctionPointerParameterListSyntax node)
        {
            HookActions(WellKnownSyntax.FunctionPointerParameterListSyntax, node, () => base.VisitFunctionPointerParameterList(node));
        }

        public override void VisitFunctionPointerCallingConvention(FunctionPointerCallingConventionSyntax node)
        {
            HookActions(WellKnownSyntax.FunctionPointerCallingConventionSyntax, node, () => base.VisitFunctionPointerCallingConvention(node));
        }

        public override void VisitFunctionPointerUnmanagedCallingConventionList(FunctionPointerUnmanagedCallingConventionListSyntax node)
        {
            HookActions(WellKnownSyntax.FunctionPointerUnmanagedCallingConventionListSyntax, node, () => base.VisitFunctionPointerUnmanagedCallingConventionList(node));
        }

        public override void VisitFunctionPointerUnmanagedCallingConvention(FunctionPointerUnmanagedCallingConventionSyntax node)
        {
            HookActions(WellKnownSyntax.FunctionPointerUnmanagedCallingConventionSyntax, node, () => base.VisitFunctionPointerUnmanagedCallingConvention(node));
        }

        public override void VisitNullableType(NullableTypeSyntax node)
        {
            HookActions(WellKnownSyntax.NullableTypeSyntax, node, () => base.VisitNullableType(node));
        }

        public override void VisitTupleType(TupleTypeSyntax node)
        {
            HookActions(WellKnownSyntax.TupleTypeSyntax, node, () => base.VisitTupleType(node));
        }

        public override void VisitTupleElement(TupleElementSyntax node)
        {
            HookActions(WellKnownSyntax.TupleElementSyntax, node, () => base.VisitTupleElement(node));
        }

        public override void VisitOmittedTypeArgument(OmittedTypeArgumentSyntax node)
        {
            HookActions(WellKnownSyntax.OmittedTypeArgumentSyntax, node, () => base.VisitOmittedTypeArgument(node));
        }

        public override void VisitRefType(RefTypeSyntax node)
        {
            HookActions(WellKnownSyntax.RefTypeSyntax, node, () => base.VisitRefType(node));
        }

        public override void VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.ParenthesizedExpressionSyntax, node, () => base.VisitParenthesizedExpression(node));
        }

        public override void VisitTupleExpression(TupleExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.TupleExpressionSyntax, node, () => base.VisitTupleExpression(node));
        }

        public override void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.PrefixUnaryExpressionSyntax, node, () => base.VisitPrefixUnaryExpression(node));
        }

        public override void VisitAwaitExpression(AwaitExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.AwaitExpressionSyntax, node, () => base.VisitAwaitExpression(node));
        }

        public override void VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.PostfixUnaryExpressionSyntax, node, () => base.VisitPostfixUnaryExpression(node));
        }

        public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.MemberAccessExpressionSyntax, node, () => base.VisitMemberAccessExpression(node));
        }

        public override void VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.ConditionalAccessExpressionSyntax, node, () => base.VisitConditionalAccessExpression(node));
        }

        public override void VisitMemberBindingExpression(MemberBindingExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.MemberBindingExpressionSyntax, node, () => base.VisitMemberBindingExpression(node));
        }

        public override void VisitElementBindingExpression(ElementBindingExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.ElementBindingExpressionSyntax, node, () => base.VisitElementBindingExpression(node));
        }

        public override void VisitRangeExpression(RangeExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.RangeExpressionSyntax, node, () => base.VisitRangeExpression(node));
        }

        public override void VisitImplicitElementAccess(ImplicitElementAccessSyntax node)
        {
            HookActions(WellKnownSyntax.ImplicitElementAccessSyntax, node, () => base.VisitImplicitElementAccess(node));
        }

        public override void VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.BinaryExpressionSyntax, node, () => base.VisitBinaryExpression(node));
        }

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.AssignmentExpressionSyntax, node, () => base.VisitAssignmentExpression(node));
        }

        public override void VisitConditionalExpression(ConditionalExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.ConditionalExpressionSyntax, node, () => base.VisitConditionalExpression(node));
        }

        public override void VisitThisExpression(ThisExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.ThisExpressionSyntax, node, () => base.VisitThisExpression(node));
        }

        public override void VisitBaseExpression(BaseExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.BaseExpressionSyntax, node, () => base.VisitBaseExpression(node));
        }

        public override void VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.LiteralExpressionSyntax, node, () => base.VisitLiteralExpression(node));
        }

        public override void VisitMakeRefExpression(MakeRefExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.MakeRefExpressionSyntax, node, () => base.VisitMakeRefExpression(node));
        }

        public override void VisitRefTypeExpression(RefTypeExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.RefTypeExpressionSyntax, node, () => base.VisitRefTypeExpression(node));
        }

        public override void VisitRefValueExpression(RefValueExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.RefValueExpressionSyntax, node, () => base.VisitRefValueExpression(node));
        }

        public override void VisitCheckedExpression(CheckedExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.CheckedExpressionSyntax, node, () => base.VisitCheckedExpression(node));
        }

        public override void VisitDefaultExpression(DefaultExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.DefaultExpressionSyntax, node, () => base.VisitDefaultExpression(node));
        }

        public override void VisitTypeOfExpression(TypeOfExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.TypeOfExpressionSyntax, node, () => base.VisitTypeOfExpression(node));
        }

        public override void VisitSizeOfExpression(SizeOfExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.SizeOfExpressionSyntax, node, () => base.VisitSizeOfExpression(node));
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.InvocationExpressionSyntax, node, () => base.VisitInvocationExpression(node));
        }

        public override void VisitElementAccessExpression(ElementAccessExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.ElementAccessExpressionSyntax, node, () => base.VisitElementAccessExpression(node));
        }

        public override void VisitArgumentList(ArgumentListSyntax node)
        {
            HookActions(WellKnownSyntax.ArgumentListSyntax, node, () => base.VisitArgumentList(node));
        }

        public override void VisitBracketedArgumentList(BracketedArgumentListSyntax node)
        {
            HookActions(WellKnownSyntax.BracketedArgumentListSyntax, node, () => base.VisitBracketedArgumentList(node));
        }

        public override void VisitArgument(ArgumentSyntax node)
        {
            HookActions(WellKnownSyntax.ArgumentSyntax, node, () => base.VisitArgument(node));
        }

        public override void VisitNameColon(NameColonSyntax node)
        {
            HookActions(WellKnownSyntax.NameColonSyntax, node, () => base.VisitNameColon(node));
        }

        public override void VisitDeclarationExpression(DeclarationExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.DeclarationExpressionSyntax, node, () => base.VisitDeclarationExpression(node));
        }

        public override void VisitCastExpression(CastExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.CastExpressionSyntax, node, () => base.VisitCastExpression(node));
        }

        public override void VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.AnonymousMethodExpressionSyntax, node, () => base.VisitAnonymousMethodExpression(node));
        }

        public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.SimpleLambdaExpressionSyntax, node, () => base.VisitSimpleLambdaExpression(node));
        }

        public override void VisitRefExpression(RefExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.RefExpressionSyntax, node, () => base.VisitRefExpression(node));
        }

        public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.ParenthesizedLambdaExpressionSyntax, node, () => base.VisitParenthesizedLambdaExpression(node));
        }

        public override void VisitInitializerExpression(InitializerExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.InitializerExpressionSyntax, node, () => base.VisitInitializerExpression(node));
        }

        public override void VisitImplicitObjectCreationExpression(ImplicitObjectCreationExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.ImplicitObjectCreationExpressionSyntax, node, () => base.VisitImplicitObjectCreationExpression(node));
        }

        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.ObjectCreationExpressionSyntax, node, () => base.VisitObjectCreationExpression(node));
        }

        public override void VisitWithExpression(WithExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.WithExpressionSyntax, node, () => base.VisitWithExpression(node));
        }

        public override void VisitAnonymousObjectMemberDeclarator(AnonymousObjectMemberDeclaratorSyntax node)
        {
            HookActions(WellKnownSyntax.AnonymousObjectMemberDeclaratorSyntax, node, () => base.VisitAnonymousObjectMemberDeclarator(node));
        }

        public override void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.AnonymousObjectCreationExpressionSyntax, node, () => base.VisitAnonymousObjectCreationExpression(node));
        }

        public override void VisitArrayCreationExpression(ArrayCreationExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.ArrayCreationExpressionSyntax, node, () => base.VisitArrayCreationExpression(node));
        }

        public override void VisitImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.ImplicitArrayCreationExpressionSyntax, node, () => base.VisitImplicitArrayCreationExpression(node));
        }

        public override void VisitStackAllocArrayCreationExpression(StackAllocArrayCreationExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.StackAllocArrayCreationExpressionSyntax, node, () => base.VisitStackAllocArrayCreationExpression(node));
        }

        public override void VisitImplicitStackAllocArrayCreationExpression(ImplicitStackAllocArrayCreationExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.ImplicitStackAllocArrayCreationExpressionSyntax, node, () => base.VisitImplicitStackAllocArrayCreationExpression(node));
        }

        public override void VisitQueryExpression(QueryExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.QueryExpressionSyntax, node, () => base.VisitQueryExpression(node));
        }

        public override void VisitQueryBody(QueryBodySyntax node)
        {
            HookActions(WellKnownSyntax.QueryBodySyntax, node, () => base.VisitQueryBody(node));
        }

        public override void VisitFromClause(FromClauseSyntax node)
        {
            HookActions(WellKnownSyntax.FromClauseSyntax, node, () => base.VisitFromClause(node));
        }

        public override void VisitLetClause(LetClauseSyntax node)
        {
            HookActions(WellKnownSyntax.LetClauseSyntax, node, () => base.VisitLetClause(node));
        }

        public override void VisitJoinClause(JoinClauseSyntax node)
        {
            HookActions(WellKnownSyntax.JoinClauseSyntax, node, () => base.VisitJoinClause(node));
        }

        public override void VisitJoinIntoClause(JoinIntoClauseSyntax node)
        {
            HookActions(WellKnownSyntax.JoinIntoClauseSyntax, node, () => base.VisitJoinIntoClause(node));
        }

        public override void VisitWhereClause(WhereClauseSyntax node)
        {
            HookActions(WellKnownSyntax.WhereClauseSyntax, node, () => base.VisitWhereClause(node));
        }

        public override void VisitOrderByClause(OrderByClauseSyntax node)
        {
            HookActions(WellKnownSyntax.OrderByClauseSyntax, node, () => base.VisitOrderByClause(node));
        }

        public override void VisitOrdering(OrderingSyntax node)
        {
            HookActions(WellKnownSyntax.OrderingSyntax, node, () => base.VisitOrdering(node));
        }

        public override void VisitSelectClause(SelectClauseSyntax node)
        {
            HookActions(WellKnownSyntax.SelectClauseSyntax, node, () => base.VisitSelectClause(node));
        }

        public override void VisitGroupClause(GroupClauseSyntax node)
        {
            HookActions(WellKnownSyntax.GroupClauseSyntax, node, () => base.VisitGroupClause(node));
        }

        public override void VisitQueryContinuation(QueryContinuationSyntax node)
        {
            HookActions(WellKnownSyntax.QueryContinuationSyntax, node, () => base.VisitQueryContinuation(node));
        }

        public override void VisitOmittedArraySizeExpression(OmittedArraySizeExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.OmittedArraySizeExpressionSyntax, node, () => base.VisitOmittedArraySizeExpression(node));
        }

        public override void VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.InterpolatedStringExpressionSyntax, node, () => base.VisitInterpolatedStringExpression(node));
        }

        public override void VisitIsPatternExpression(IsPatternExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.IsPatternExpressionSyntax, node, () => base.VisitIsPatternExpression(node));
        }

        public override void VisitThrowExpression(ThrowExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.ThrowExpressionSyntax, node, () => base.VisitThrowExpression(node));
        }

        public override void VisitWhenClause(WhenClauseSyntax node)
        {
            HookActions(WellKnownSyntax.WhenClauseSyntax, node, () => base.VisitWhenClause(node));
        }

        public override void VisitDiscardPattern(DiscardPatternSyntax node)
        {
            HookActions(WellKnownSyntax.DiscardPatternSyntax, node, () => base.VisitDiscardPattern(node));
        }

        public override void VisitDeclarationPattern(DeclarationPatternSyntax node)
        {
            HookActions(WellKnownSyntax.DeclarationPatternSyntax, node, () => base.VisitDeclarationPattern(node));
        }

        public override void VisitVarPattern(VarPatternSyntax node)
        {
            HookActions(WellKnownSyntax.VarPatternSyntax, node, () => base.VisitVarPattern(node));
        }

        public override void VisitRecursivePattern(RecursivePatternSyntax node)
        {
            HookActions(WellKnownSyntax.RecursivePatternSyntax, node, () => base.VisitRecursivePattern(node));
        }

        public override void VisitPositionalPatternClause(PositionalPatternClauseSyntax node)
        {
            HookActions(WellKnownSyntax.PositionalPatternClauseSyntax, node, () => base.VisitPositionalPatternClause(node));
        }

        public override void VisitPropertyPatternClause(PropertyPatternClauseSyntax node)
        {
            HookActions(WellKnownSyntax.PropertyPatternClauseSyntax, node, () => base.VisitPropertyPatternClause(node));
        }

        public override void VisitSubpattern(SubpatternSyntax node)
        {
            HookActions(WellKnownSyntax.SubpatternSyntax, node, () => base.VisitSubpattern(node));
        }

        public override void VisitConstantPattern(ConstantPatternSyntax node)
        {
            HookActions(WellKnownSyntax.ConstantPatternSyntax, node, () => base.VisitConstantPattern(node));
        }

        public override void VisitParenthesizedPattern(ParenthesizedPatternSyntax node)
        {
            HookActions(WellKnownSyntax.ParenthesizedPatternSyntax, node, () => base.VisitParenthesizedPattern(node));
        }

        public override void VisitRelationalPattern(RelationalPatternSyntax node)
        {
            HookActions(WellKnownSyntax.RelationalPatternSyntax, node, () => base.VisitRelationalPattern(node));
        }

        public override void VisitTypePattern(TypePatternSyntax node)
        {
            HookActions(WellKnownSyntax.TypePatternSyntax, node, () => base.VisitTypePattern(node));
        }

        public override void VisitBinaryPattern(BinaryPatternSyntax node)
        {
            HookActions(WellKnownSyntax.BinaryPatternSyntax, node, () => base.VisitBinaryPattern(node));
        }

        public override void VisitUnaryPattern(UnaryPatternSyntax node)
        {
            HookActions(WellKnownSyntax.UnaryPatternSyntax, node, () => base.VisitUnaryPattern(node));
        }

        public override void VisitInterpolatedStringText(InterpolatedStringTextSyntax node)
        {
            HookActions(WellKnownSyntax.InterpolatedStringTextSyntax, node, () => base.VisitInterpolatedStringText(node));
        }

        public override void VisitInterpolation(InterpolationSyntax node)
        {
            HookActions(WellKnownSyntax.InterpolationSyntax, node, () => base.VisitInterpolation(node));
        }

        public override void VisitInterpolationAlignmentClause(InterpolationAlignmentClauseSyntax node)
        {
            HookActions(WellKnownSyntax.InterpolationAlignmentClauseSyntax, node, () => base.VisitInterpolationAlignmentClause(node));
        }

        public override void VisitInterpolationFormatClause(InterpolationFormatClauseSyntax node)
        {
            HookActions(WellKnownSyntax.InterpolationFormatClauseSyntax, node, () => base.VisitInterpolationFormatClause(node));
        }

        public override void VisitGlobalStatement(GlobalStatementSyntax node)
        {
            HookActions(WellKnownSyntax.GlobalStatementSyntax, node, () => base.VisitGlobalStatement(node));
        }

        public override void VisitBlock(BlockSyntax node)
        {
            HookActions(WellKnownSyntax.BlockSyntax, node, () => base.VisitBlock(node));
        }

        public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
        {
            HookActions(WellKnownSyntax.LocalFunctionStatementSyntax, node, () => base.VisitLocalFunctionStatement(node));
        }

        public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            HookActions(WellKnownSyntax.LocalDeclarationStatementSyntax, node, () => base.VisitLocalDeclarationStatement(node));
        }

        public override void VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            HookActions(WellKnownSyntax.VariableDeclarationSyntax, node, () => base.VisitVariableDeclaration(node));
        }

        public override void VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            HookActions(WellKnownSyntax.VariableDeclaratorSyntax, node, () => base.VisitVariableDeclarator(node));
        }

        public override void VisitEqualsValueClause(EqualsValueClauseSyntax node)
        {
            HookActions(WellKnownSyntax.EqualsValueClauseSyntax, node, () => base.VisitEqualsValueClause(node));
        }

        public override void VisitSingleVariableDesignation(SingleVariableDesignationSyntax node)
        {
            HookActions(WellKnownSyntax.SingleVariableDesignationSyntax, node, () => base.VisitSingleVariableDesignation(node));
        }

        public override void VisitDiscardDesignation(DiscardDesignationSyntax node)
        {
            HookActions(WellKnownSyntax.DiscardDesignationSyntax, node, () => base.VisitDiscardDesignation(node));
        }

        public override void VisitParenthesizedVariableDesignation(ParenthesizedVariableDesignationSyntax node)
        {
            HookActions(WellKnownSyntax.ParenthesizedVariableDesignationSyntax, node, () => base.VisitParenthesizedVariableDesignation(node));
        }

        public override void VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            HookActions(WellKnownSyntax.ExpressionStatementSyntax, node, () => base.VisitExpressionStatement(node));
        }

        public override void VisitEmptyStatement(EmptyStatementSyntax node)
        {
            HookActions(WellKnownSyntax.EmptyStatementSyntax, node, () => base.VisitEmptyStatement(node));
        }

        public override void VisitLabeledStatement(LabeledStatementSyntax node)
        {
            HookActions(WellKnownSyntax.LabeledStatementSyntax, node, () => base.VisitLabeledStatement(node));
        }

        public override void VisitGotoStatement(GotoStatementSyntax node)
        {
            HookActions(WellKnownSyntax.GotoStatementSyntax, node, () => base.VisitGotoStatement(node));
        }

        public override void VisitBreakStatement(BreakStatementSyntax node)
        {
            HookActions(WellKnownSyntax.BreakStatementSyntax, node, () => base.VisitBreakStatement(node));
        }

        public override void VisitContinueStatement(ContinueStatementSyntax node)
        {
            HookActions(WellKnownSyntax.ContinueStatementSyntax, node, () => base.VisitContinueStatement(node));
        }

        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
            HookActions(WellKnownSyntax.ReturnStatementSyntax, node, () => base.VisitReturnStatement(node));
        }

        public override void VisitThrowStatement(ThrowStatementSyntax node)
        {
            HookActions(WellKnownSyntax.ThrowStatementSyntax, node, () => base.VisitThrowStatement(node));
        }

        public override void VisitYieldStatement(YieldStatementSyntax node)
        {
            HookActions(WellKnownSyntax.YieldStatementSyntax, node, () => base.VisitYieldStatement(node));
        }

        public override void VisitWhileStatement(WhileStatementSyntax node)
        {
            HookActions(WellKnownSyntax.WhileStatementSyntax, node, () => base.VisitWhileStatement(node));
        }

        public override void VisitDoStatement(DoStatementSyntax node)
        {
            HookActions(WellKnownSyntax.DoStatementSyntax, node, () => base.VisitDoStatement(node));
        }

        public override void VisitForStatement(ForStatementSyntax node)
        {
            HookActions(WellKnownSyntax.ForStatementSyntax, node, () => base.VisitForStatement(node));
        }

        public override void VisitForEachStatement(ForEachStatementSyntax node)
        {
            HookActions(WellKnownSyntax.ForEachStatementSyntax, node, () => base.VisitForEachStatement(node));
        }

        public override void VisitForEachVariableStatement(ForEachVariableStatementSyntax node)
        {
            HookActions(WellKnownSyntax.ForEachVariableStatementSyntax, node, () => base.VisitForEachVariableStatement(node));
        }

        public override void VisitUsingStatement(UsingStatementSyntax node)
        {
            HookActions(WellKnownSyntax.UsingStatementSyntax, node, () => base.VisitUsingStatement(node));
        }

        public override void VisitFixedStatement(FixedStatementSyntax node)
        {
            HookActions(WellKnownSyntax.FixedStatementSyntax, node, () => base.VisitFixedStatement(node));
        }

        public override void VisitCheckedStatement(CheckedStatementSyntax node)
        {
            HookActions(WellKnownSyntax.CheckedStatementSyntax, node, () => base.VisitCheckedStatement(node));
        }

        public override void VisitUnsafeStatement(UnsafeStatementSyntax node)
        {
            HookActions(WellKnownSyntax.UnsafeStatementSyntax, node, () => base.VisitUnsafeStatement(node));
        }

        public override void VisitLockStatement(LockStatementSyntax node)
        {
            HookActions(WellKnownSyntax.LockStatementSyntax, node, () => base.VisitLockStatement(node));
        }

        public override void VisitIfStatement(IfStatementSyntax node)
        {
            HookActions(WellKnownSyntax.IfStatementSyntax, node, () => base.VisitIfStatement(node));
        }

        public override void VisitElseClause(ElseClauseSyntax node)
        {
            HookActions(WellKnownSyntax.ElseClauseSyntax, node, () => base.VisitElseClause(node));
        }

        public override void VisitSwitchStatement(SwitchStatementSyntax node)
        {
            HookActions(WellKnownSyntax.SwitchStatementSyntax, node, () => base.VisitSwitchStatement(node));
        }

        public override void VisitSwitchSection(SwitchSectionSyntax node)
        {
            HookActions(WellKnownSyntax.SwitchSectionSyntax, node, () => base.VisitSwitchSection(node));
        }

        public override void VisitCasePatternSwitchLabel(CasePatternSwitchLabelSyntax node)
        {
            HookActions(WellKnownSyntax.CasePatternSwitchLabelSyntax, node, () => base.VisitCasePatternSwitchLabel(node));
        }

        public override void VisitCaseSwitchLabel(CaseSwitchLabelSyntax node)
        {
            HookActions(WellKnownSyntax.CaseSwitchLabelSyntax, node, () => base.VisitCaseSwitchLabel(node));
        }

        public override void VisitDefaultSwitchLabel(DefaultSwitchLabelSyntax node)
        {
            HookActions(WellKnownSyntax.DefaultSwitchLabelSyntax, node, () => base.VisitDefaultSwitchLabel(node));
        }

        public override void VisitSwitchExpression(SwitchExpressionSyntax node)
        {
            HookActions(WellKnownSyntax.SwitchExpressionSyntax, node, () => base.VisitSwitchExpression(node));
        }

        public override void VisitSwitchExpressionArm(SwitchExpressionArmSyntax node)
        {
            HookActions(WellKnownSyntax.SwitchExpressionArmSyntax, node, () => base.VisitSwitchExpressionArm(node));
        }

        public override void VisitTryStatement(TryStatementSyntax node)
        {
            HookActions(WellKnownSyntax.TryStatementSyntax, node, () => base.VisitTryStatement(node));
        }

        public override void VisitCatchClause(CatchClauseSyntax node)
        {
            HookActions(WellKnownSyntax.CatchClauseSyntax, node, () => base.VisitCatchClause(node));
        }

        public override void VisitCatchDeclaration(CatchDeclarationSyntax node)
        {
            HookActions(WellKnownSyntax.CatchDeclarationSyntax, node, () => base.VisitCatchDeclaration(node));
        }

        public override void VisitCatchFilterClause(CatchFilterClauseSyntax node)
        {
            HookActions(WellKnownSyntax.CatchFilterClauseSyntax, node, () => base.VisitCatchFilterClause(node));
        }

        public override void VisitFinallyClause(FinallyClauseSyntax node)
        {
            HookActions(WellKnownSyntax.FinallyClauseSyntax, node, () => base.VisitFinallyClause(node));
        }

        public override void VisitCompilationUnit(CompilationUnitSyntax node)
        {
            HookActions(WellKnownSyntax.CompilationUnitSyntax, node, () => base.VisitCompilationUnit(node));
        }

        public override void VisitExternAliasDirective(ExternAliasDirectiveSyntax node)
        {
            HookActions(WellKnownSyntax.ExternAliasDirectiveSyntax, node, () => base.VisitExternAliasDirective(node));
        }

        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            HookActions(WellKnownSyntax.UsingDirectiveSyntax, node, () => base.VisitUsingDirective(node));
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            HookActions(WellKnownSyntax.NamespaceDeclarationSyntax, node, () => base.VisitNamespaceDeclaration(node));
        }

        public override void VisitAttributeList(AttributeListSyntax node)
        {
            HookActions(WellKnownSyntax.AttributeListSyntax, node, () => base.VisitAttributeList(node));
        }

        public override void VisitAttributeTargetSpecifier(AttributeTargetSpecifierSyntax node)
        {
            HookActions(WellKnownSyntax.AttributeTargetSpecifierSyntax, node, () => base.VisitAttributeTargetSpecifier(node));
        }

        public override void VisitAttribute(AttributeSyntax node)
        {
            HookActions(WellKnownSyntax.AttributeSyntax, node, () => base.VisitAttribute(node));
        }

        public override void VisitAttributeArgumentList(AttributeArgumentListSyntax node)
        {
            HookActions(WellKnownSyntax.AttributeArgumentListSyntax, node, () => base.VisitAttributeArgumentList(node));
        }

        public override void VisitAttributeArgument(AttributeArgumentSyntax node)
        {
            HookActions(WellKnownSyntax.AttributeArgumentSyntax, node, () => base.VisitAttributeArgument(node));
        }

        public override void VisitNameEquals(NameEqualsSyntax node)
        {
            HookActions(WellKnownSyntax.NameEqualsSyntax, node, () => base.VisitNameEquals(node));
        }

        public override void VisitTypeParameterList(TypeParameterListSyntax node)
        {
            HookActions(WellKnownSyntax.TypeParameterListSyntax, node, () => base.VisitTypeParameterList(node));
        }

        public override void VisitTypeParameter(TypeParameterSyntax node)
        {
            HookActions(WellKnownSyntax.TypeParameterSyntax, node, () => base.VisitTypeParameter(node));
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            HookActions(WellKnownSyntax.ClassDeclarationSyntax, node, () => base.VisitClassDeclaration(node));
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            HookActions(WellKnownSyntax.StructDeclarationSyntax, node, () => base.VisitStructDeclaration(node));
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            HookActions(WellKnownSyntax.InterfaceDeclarationSyntax, node, () => base.VisitInterfaceDeclaration(node));
        }

        public override void VisitRecordDeclaration(RecordDeclarationSyntax node)
        {
            HookActions(WellKnownSyntax.RecordDeclarationSyntax, node, () => base.VisitRecordDeclaration(node));
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            HookActions(WellKnownSyntax.EnumDeclarationSyntax, node, () => base.VisitEnumDeclaration(node));
        }

        public override void VisitDelegateDeclaration(DelegateDeclarationSyntax node)
        {
            HookActions(WellKnownSyntax.DelegateDeclarationSyntax, node, () => base.VisitDelegateDeclaration(node));
        }

        public override void VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
        {
            HookActions(WellKnownSyntax.EnumMemberDeclarationSyntax, node, () => base.VisitEnumMemberDeclaration(node));
        }

        public override void VisitBaseList(BaseListSyntax node)
        {
            HookActions(WellKnownSyntax.BaseListSyntax, node, () => base.VisitBaseList(node));
        }

        public override void VisitSimpleBaseType(SimpleBaseTypeSyntax node)
        {
            HookActions(WellKnownSyntax.SimpleBaseTypeSyntax, node, () => base.VisitSimpleBaseType(node));
        }

        public override void VisitPrimaryConstructorBaseType(PrimaryConstructorBaseTypeSyntax node)
        {
            HookActions(WellKnownSyntax.PrimaryConstructorBaseTypeSyntax, node, () => base.VisitPrimaryConstructorBaseType(node));
        }

        public override void VisitTypeParameterConstraintClause(TypeParameterConstraintClauseSyntax node)
        {
            HookActions(WellKnownSyntax.TypeParameterConstraintClauseSyntax, node, () => base.VisitTypeParameterConstraintClause(node));
        }

        public override void VisitConstructorConstraint(ConstructorConstraintSyntax node)
        {
            HookActions(WellKnownSyntax.ConstructorConstraintSyntax, node, () => base.VisitConstructorConstraint(node));
        }

        public override void VisitClassOrStructConstraint(ClassOrStructConstraintSyntax node)
        {
            HookActions(WellKnownSyntax.ClassOrStructConstraintSyntax, node, () => base.VisitClassOrStructConstraint(node));
        }

        public override void VisitTypeConstraint(TypeConstraintSyntax node)
        {
            HookActions(WellKnownSyntax.TypeConstraintSyntax, node, () => base.VisitTypeConstraint(node));
        }

        public override void VisitDefaultConstraint(DefaultConstraintSyntax node)
        {
            HookActions(WellKnownSyntax.DefaultConstraintSyntax, node, () => base.VisitDefaultConstraint(node));
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            HookActions(WellKnownSyntax.FieldDeclarationSyntax, node, () => base.VisitFieldDeclaration(node));
        }

        public override void VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
            HookActions(WellKnownSyntax.EventFieldDeclarationSyntax, node, () => base.VisitEventFieldDeclaration(node));
        }

        public override void VisitExplicitInterfaceSpecifier(ExplicitInterfaceSpecifierSyntax node)
        {
            HookActions(WellKnownSyntax.ExplicitInterfaceSpecifierSyntax, node, () => base.VisitExplicitInterfaceSpecifier(node));
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            HookActions(WellKnownSyntax.MethodDeclarationSyntax, node, () => base.VisitMethodDeclaration(node));
        }

        public override void VisitOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            HookActions(WellKnownSyntax.OperatorDeclarationSyntax, node, () => base.VisitOperatorDeclaration(node));
        }

        public override void VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
        {
            HookActions(WellKnownSyntax.ConversionOperatorDeclarationSyntax, node, () => base.VisitConversionOperatorDeclaration(node));
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            HookActions(WellKnownSyntax.ConstructorDeclarationSyntax, node, () => base.VisitConstructorDeclaration(node));
        }

        public override void VisitConstructorInitializer(ConstructorInitializerSyntax node)
        {
            HookActions(WellKnownSyntax.ConstructorInitializerSyntax, node, () => base.VisitConstructorInitializer(node));
        }

        public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
        {
            HookActions(WellKnownSyntax.DestructorDeclarationSyntax, node, () => base.VisitDestructorDeclaration(node));
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            HookActions(WellKnownSyntax.PropertyDeclarationSyntax, node, () => base.VisitPropertyDeclaration(node));
        }

        public override void VisitArrowExpressionClause(ArrowExpressionClauseSyntax node)
        {
            HookActions(WellKnownSyntax.ArrowExpressionClauseSyntax, node, () => base.VisitArrowExpressionClause(node));
        }

        public override void VisitEventDeclaration(EventDeclarationSyntax node)
        {
            HookActions(WellKnownSyntax.EventDeclarationSyntax, node, () => base.VisitEventDeclaration(node));
        }

        public override void VisitIndexerDeclaration(IndexerDeclarationSyntax node)
        {
            HookActions(WellKnownSyntax.IndexerDeclarationSyntax, node, () => base.VisitIndexerDeclaration(node));
        }

        public override void VisitAccessorList(AccessorListSyntax node)
        {
            HookActions(WellKnownSyntax.AccessorListSyntax, node, () => base.VisitAccessorList(node));
        }

        public override void VisitAccessorDeclaration(AccessorDeclarationSyntax node)
        {
            HookActions(WellKnownSyntax.AccessorDeclarationSyntax, node, () => base.VisitAccessorDeclaration(node));
        }

        public override void VisitParameterList(ParameterListSyntax node)
        {
            HookActions(WellKnownSyntax.ParameterListSyntax, node, () => base.VisitParameterList(node));
        }

        public override void VisitBracketedParameterList(BracketedParameterListSyntax node)
        {
            HookActions(WellKnownSyntax.BracketedParameterListSyntax, node, () => base.VisitBracketedParameterList(node));
        }

        public override void VisitParameter(ParameterSyntax node)
        {
            HookActions(WellKnownSyntax.ParameterSyntax, node, () => base.VisitParameter(node));
        }

        public override void VisitFunctionPointerParameter(FunctionPointerParameterSyntax node)
        {
            HookActions(WellKnownSyntax.FunctionPointerParameterSyntax, node, () => base.VisitFunctionPointerParameter(node));
        }

        public override void VisitIncompleteMember(IncompleteMemberSyntax node)
        {
            HookActions(WellKnownSyntax.IncompleteMemberSyntax, node, () => base.VisitIncompleteMember(node));
        }

        public override void VisitSkippedTokensTrivia(SkippedTokensTriviaSyntax node)
        {
            HookActions(WellKnownSyntax.SkippedTokensTriviaSyntax, node, () => base.VisitSkippedTokensTrivia(node));
        }

        public override void VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node)
        {
            HookActions(WellKnownSyntax.DocumentationCommentTriviaSyntax, node, () => base.VisitDocumentationCommentTrivia(node));
        }

        public override void VisitTypeCref(TypeCrefSyntax node)
        {
            HookActions(WellKnownSyntax.TypeCrefSyntax, node, () => base.VisitTypeCref(node));
        }

        public override void VisitQualifiedCref(QualifiedCrefSyntax node)
        {
            HookActions(WellKnownSyntax.QualifiedCrefSyntax, node, () => base.VisitQualifiedCref(node));
        }

        public override void VisitNameMemberCref(NameMemberCrefSyntax node)
        {
            HookActions(WellKnownSyntax.NameMemberCrefSyntax, node, () => base.VisitNameMemberCref(node));
        }

        public override void VisitIndexerMemberCref(IndexerMemberCrefSyntax node)
        {
            HookActions(WellKnownSyntax.IndexerMemberCrefSyntax, node, () => base.VisitIndexerMemberCref(node));
        }

        public override void VisitOperatorMemberCref(OperatorMemberCrefSyntax node)
        {
            HookActions(WellKnownSyntax.OperatorMemberCrefSyntax, node, () => base.VisitOperatorMemberCref(node));
        }

        public override void VisitConversionOperatorMemberCref(ConversionOperatorMemberCrefSyntax node)
        {
            HookActions(WellKnownSyntax.ConversionOperatorMemberCrefSyntax, node, () => base.VisitConversionOperatorMemberCref(node));
        }

        public override void VisitCrefParameterList(CrefParameterListSyntax node)
        {
            HookActions(WellKnownSyntax.CrefParameterListSyntax, node, () => base.VisitCrefParameterList(node));
        }

        public override void VisitCrefBracketedParameterList(CrefBracketedParameterListSyntax node)
        {
            HookActions(WellKnownSyntax.CrefBracketedParameterListSyntax, node, () => base.VisitCrefBracketedParameterList(node));
        }

        public override void VisitCrefParameter(CrefParameterSyntax node)
        {
            HookActions(WellKnownSyntax.CrefParameterSyntax, node, () => base.VisitCrefParameter(node));
        }

        public override void VisitXmlElement(XmlElementSyntax node)
        {
            HookActions(WellKnownSyntax.XmlElementSyntax, node, () => base.VisitXmlElement(node));
        }

        public override void VisitXmlElementStartTag(XmlElementStartTagSyntax node)
        {
            HookActions(WellKnownSyntax.XmlElementStartTagSyntax, node, () => base.VisitXmlElementStartTag(node));
        }

        public override void VisitXmlElementEndTag(XmlElementEndTagSyntax node)
        {
            HookActions(WellKnownSyntax.XmlElementEndTagSyntax, node, () => base.VisitXmlElementEndTag(node));
        }

        public override void VisitXmlEmptyElement(XmlEmptyElementSyntax node)
        {
            HookActions(WellKnownSyntax.XmlEmptyElementSyntax, node, () => base.VisitXmlEmptyElement(node));
        }

        public override void VisitXmlName(XmlNameSyntax node)
        {
            HookActions(WellKnownSyntax.XmlNameSyntax, node, () => base.VisitXmlName(node));
        }

        public override void VisitXmlPrefix(XmlPrefixSyntax node)
        {
            HookActions(WellKnownSyntax.XmlPrefixSyntax, node, () => base.VisitXmlPrefix(node));
        }

        public override void VisitXmlTextAttribute(XmlTextAttributeSyntax node)
        {
            HookActions(WellKnownSyntax.XmlTextAttributeSyntax, node, () => base.VisitXmlTextAttribute(node));
        }

        public override void VisitXmlCrefAttribute(XmlCrefAttributeSyntax node)
        {
            HookActions(WellKnownSyntax.XmlCrefAttributeSyntax, node, () => base.VisitXmlCrefAttribute(node));
        }

        public override void VisitXmlNameAttribute(XmlNameAttributeSyntax node)
        {
            HookActions(WellKnownSyntax.XmlNameAttributeSyntax, node, () => base.VisitXmlNameAttribute(node));
        }

        public override void VisitXmlText(XmlTextSyntax node)
        {
            HookActions(WellKnownSyntax.XmlTextSyntax, node, () => base.VisitXmlText(node));
        }

        public override void VisitXmlCDataSection(XmlCDataSectionSyntax node)
        {
            HookActions(WellKnownSyntax.XmlCDataSectionSyntax, node, () => base.VisitXmlCDataSection(node));
        }

        public override void VisitXmlProcessingInstruction(XmlProcessingInstructionSyntax node)
        {
            HookActions(WellKnownSyntax.XmlProcessingInstructionSyntax, node, () => base.VisitXmlProcessingInstruction(node));
        }

        public override void VisitXmlComment(XmlCommentSyntax node)
        {
            HookActions(WellKnownSyntax.XmlCommentSyntax, node, () => base.VisitXmlComment(node));
        }

        public override void VisitIfDirectiveTrivia(IfDirectiveTriviaSyntax node)
        {
            HookActions(WellKnownSyntax.IfDirectiveTriviaSyntax, node, () => base.VisitIfDirectiveTrivia(node));
        }

        public override void VisitElifDirectiveTrivia(ElifDirectiveTriviaSyntax node)
        {
            HookActions(WellKnownSyntax.ElifDirectiveTriviaSyntax, node, () => base.VisitElifDirectiveTrivia(node));
        }

        public override void VisitElseDirectiveTrivia(ElseDirectiveTriviaSyntax node)
        {
            HookActions(WellKnownSyntax.ElseDirectiveTriviaSyntax, node, () => base.VisitElseDirectiveTrivia(node));
        }

        public override void VisitEndIfDirectiveTrivia(EndIfDirectiveTriviaSyntax node)
        {
            HookActions(WellKnownSyntax.EndIfDirectiveTriviaSyntax, node, () => base.VisitEndIfDirectiveTrivia(node));
        }

        public override void VisitRegionDirectiveTrivia(RegionDirectiveTriviaSyntax node)
        {
            HookActions(WellKnownSyntax.RegionDirectiveTriviaSyntax, node, () => base.VisitRegionDirectiveTrivia(node));
        }

        public override void VisitEndRegionDirectiveTrivia(EndRegionDirectiveTriviaSyntax node)
        {
            HookActions(WellKnownSyntax.EndRegionDirectiveTriviaSyntax, node, () => base.VisitEndRegionDirectiveTrivia(node));
        }

        public override void VisitErrorDirectiveTrivia(ErrorDirectiveTriviaSyntax node)
        {
            HookActions(WellKnownSyntax.ErrorDirectiveTriviaSyntax, node, () => base.VisitErrorDirectiveTrivia(node));
        }

        public override void VisitWarningDirectiveTrivia(WarningDirectiveTriviaSyntax node)
        {
            HookActions(WellKnownSyntax.WarningDirectiveTriviaSyntax, node, () => base.VisitWarningDirectiveTrivia(node));
        }

        public override void VisitBadDirectiveTrivia(BadDirectiveTriviaSyntax node)
        {
            HookActions(WellKnownSyntax.BadDirectiveTriviaSyntax, node, () => base.VisitBadDirectiveTrivia(node));
        }

        public override void VisitDefineDirectiveTrivia(DefineDirectiveTriviaSyntax node)
        {
            HookActions(WellKnownSyntax.DefineDirectiveTriviaSyntax, node, () => base.VisitDefineDirectiveTrivia(node));
        }

        public override void VisitUndefDirectiveTrivia(UndefDirectiveTriviaSyntax node)
        {
            HookActions(WellKnownSyntax.UndefDirectiveTriviaSyntax, node, () => base.VisitUndefDirectiveTrivia(node));
        }

        public override void VisitLineDirectiveTrivia(LineDirectiveTriviaSyntax node)
        {
            HookActions(WellKnownSyntax.LineDirectiveTriviaSyntax, node, () => base.VisitLineDirectiveTrivia(node));
        }

        public override void VisitPragmaWarningDirectiveTrivia(PragmaWarningDirectiveTriviaSyntax node)
        {
            HookActions(WellKnownSyntax.PragmaWarningDirectiveTriviaSyntax, node, () => base.VisitPragmaWarningDirectiveTrivia(node));
        }

        public override void VisitPragmaChecksumDirectiveTrivia(PragmaChecksumDirectiveTriviaSyntax node)
        {
            HookActions(WellKnownSyntax.PragmaChecksumDirectiveTriviaSyntax, node, () => base.VisitPragmaChecksumDirectiveTrivia(node));
        }

        public override void VisitReferenceDirectiveTrivia(ReferenceDirectiveTriviaSyntax node)
        {
            HookActions(WellKnownSyntax.ReferenceDirectiveTriviaSyntax, node, () => base.VisitReferenceDirectiveTrivia(node));
        }

        public override void VisitLoadDirectiveTrivia(LoadDirectiveTriviaSyntax node)
        {
            HookActions(WellKnownSyntax.LoadDirectiveTriviaSyntax, node, () => base.VisitLoadDirectiveTrivia(node));
        }

        public override void VisitShebangDirectiveTrivia(ShebangDirectiveTriviaSyntax node)
        {
            HookActions(WellKnownSyntax.ShebangDirectiveTriviaSyntax, node, () => base.VisitShebangDirectiveTrivia(node));
        }

        public override void VisitNullableDirectiveTrivia(NullableDirectiveTriviaSyntax node)
        {
            HookActions(WellKnownSyntax.NullableDirectiveTriviaSyntax, node, () => base.VisitNullableDirectiveTrivia(node));
        }

        private void HookActions(WellKnownSyntax syntax, CSharpSyntaxNode node, Action around)
        {
            InvokePreAction(syntax, node, around);
            InvokePostAction(syntax, node);
        }

        private void InvokePreAction(WellKnownSyntax syntax, CSharpSyntaxNode node, Action defaultVisit)
        {
            var args = new LanguageSyntaxActionContext(node, _compilation, _semanticModel, _context, Visit, new AddOnlyCollection<IError>(), new AddOnlyCollection<IError>(), _assembly);
            var actions = _assembly.GetPreSyntaxActions(syntax);

            try
            {
                foreach (var action in actions.Where(action => action.Predicator.Invoke(args)))
                {
                    action.Action.Invoke(args);
                    if (args.ShouldStopPropagationIncludingSiblingActions)
                        break;
                }

                _context = args.SourceContext;
            }
            catch (Exception e)
            {
                if (Debugger.IsAttached)
                    Debug.WriteLine(e.Message);
            }
            finally
            {
                _errors.AddRange(args.Errors.Select(w => w.GetMessage()));
                _warnings.AddRange(args.Warnings.Select(w => w.GetMessage()));
            }

            if (args.ShouldUseDefaultVisit)
                defaultVisit.Invoke();

            if (args.ShouldStopPropagation)
                throw new StopPropagationException();
        }

        private void InvokePostAction(WellKnownSyntax syntax, CSharpSyntaxNode node)
        {
            var args = new LanguageSyntaxActionContext(node, _compilation, _semanticModel, _context, Visit, new AddOnlyCollection<IError>(), new AddOnlyCollection<IError>(), _assembly);
            var actions = _assembly.GetPostSyntaxActions(syntax);

            try
            {
                foreach (var action in actions.Where(action => action.Predicator.Invoke(args)))
                {
                    action.Action.Invoke(args);
                    if (args.ShouldStopPropagationIncludingSiblingActions)
                        break;
                }

                _context = args.SourceContext;
            }
            catch (Exception e)
            {
                if (Debugger.IsAttached)
                    Debug.WriteLine(e.Message);
            }
            finally
            {
                _errors.AddRange(args.Errors.Select(w => w.GetMessage()));
                _warnings.AddRange(args.Warnings.Select(w => w.GetMessage()));
            }

            if (args.ShouldStopPropagation)
                throw new StopPropagationException();
        }
    }
}