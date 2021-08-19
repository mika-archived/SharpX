using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpX.Compiler.CodeCleanup.Models;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.CodeCleanup;

internal class CodeCleanupCSharpSyntaxRewriter : CSharpSyntaxRewriter
{
    private readonly ILanguageSyntaxRewriterContext _context;
    private readonly CodeCleanupCSharpSyntaxWalker _walker;

    public CodeCleanupCSharpSyntaxRewriter(ILanguageSyntaxRewriterContext context, AllowList allows) : base(true)
    {
        _context = context;
        _walker = new CodeCleanupCSharpSyntaxWalker(_context, allows);
    }

    public override SyntaxNode? VisitCompilationUnit(CompilationUnitSyntax node)
    {
        _walker.Visit(node);

        return base.VisitCompilationUnit(node);
    }

    public override SyntaxNode? VisitVariableDeclaration(VariableDeclarationSyntax node)
    {
        var oldNode = node;

        foreach (var variable in node.Variables)
        {
            var symbol = _context.SemanticModel.GetDeclaredSymbol(variable);
            if (_walker.CanEliminate(symbol))
                oldNode = node.RemoveNode(node, SyntaxRemoveOptions.KeepNoTrivia);
        }

        return oldNode;
    }

    public override SyntaxNode? VisitFieldDeclaration(FieldDeclarationSyntax node)
    {
        var oldNode = node;

        foreach (var variable in node.Declaration.Variables)
        {
            var symbol = _context.SemanticModel.GetDeclaredSymbol(variable);
            if (_walker.CanEliminate(symbol))
                oldNode = node.RemoveNode(node, SyntaxRemoveOptions.KeepNoTrivia);
        }

        return oldNode;
    }

    public override SyntaxNode? VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        var symbol = _context.SemanticModel.GetDeclaredSymbol(node);
        if (_walker.CanEliminate(symbol))
            return node.RemoveNode(node, SyntaxRemoveOptions.KeepNoTrivia);

        return base.VisitMethodDeclaration(node);
    }

    public override SyntaxNode? VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        var symbol = _context.SemanticModel.GetDeclaredSymbol(node);
        if (_walker.CanEliminate(symbol))
            return node.RemoveNode(node, SyntaxRemoveOptions.KeepNoTrivia);

        return base.VisitPropertyDeclaration(node);
    }
}