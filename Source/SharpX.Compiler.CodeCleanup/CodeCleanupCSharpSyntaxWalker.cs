using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;

using SharpX.Compiler.CodeCleanup.Models;
using SharpX.Compiler.Composition.Interfaces;
using SharpX.Compiler.Extensions;
using SharpX.Library.CodeCleanup.Attributes;

namespace SharpX.Compiler.CodeCleanup;

internal class CodeCleanupCSharpSyntaxWalker : CSharpSyntaxWalker
{
    private readonly AllowList _allows;
    private readonly ILanguageSyntaxRewriterContext _context;
    private readonly Dictionary<string, bool> _signatureDictionary;
    private readonly List<string> _attributes;

    public CodeCleanupCSharpSyntaxWalker(ILanguageSyntaxRewriterContext context, AllowList allows) : base(SyntaxWalkerDepth.Token)
    {
        _context = context;
        _allows = allows;
        _attributes = new List<string>(_allows.Attributes);
        _signatureDictionary = new Dictionary<string, bool>();
    }

    public bool CanEliminate(ISymbol? symbol)
    {
        if (symbol == null)
            return false;

        if (_signatureDictionary.ContainsKey(symbol.ToUniqueSignature()))
            return _signatureDictionary[symbol.ToUniqueSignature()];

        return false;
    }

    private bool HasAttributes(ISymbol? symbol)
    {
        foreach (var attribute in _attributes)
        {
            if (symbol.HasAttribute(attribute, _context.SemanticModel))
                return true;
        }

        return false;
    }

    public override void VisitVariableDeclaration(VariableDeclarationSyntax node)
    {
        var symbol = _context.SemanticModel.GetDeclaredSymbol(node);
        if (symbol != null)
        {
            var references = SymbolFinder.FindReferencesAsync(symbol, _context.Solution).Result;
            var s = references.SelectMany(w => w.Locations).Count();
            if (s == 0)
                _signatureDictionary.Add(symbol.ToUniqueSignature(), !_allows.Variables.Contains(symbol.ToUniqueSignature()));
        }

        base.VisitVariableDeclaration(node);
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var symbol = _context.SemanticModel.GetDeclaredSymbol(node);
        if (symbol.HasAttribute<StubAttribute>(_context.SemanticModel))
            return;

        base.VisitClassDeclaration(node);
    }

    public override void VisitStructDeclaration(StructDeclarationSyntax node)
    {
        var symbol = _context.SemanticModel.GetDeclaredSymbol(node);
        if (symbol.HasAttribute<StubAttribute>(_context.SemanticModel))
            return;

        base.VisitStructDeclaration(node);
    }

    public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
    {
        var symbol = _context.SemanticModel.GetDeclaredSymbol(node);
        if (symbol.HasAttribute<StubAttribute>(_context.SemanticModel))
            return;

        base.VisitInterfaceDeclaration(node);
    }

    public override void VisitRecordDeclaration(RecordDeclarationSyntax node)
    {
        var symbol = _context.SemanticModel.GetDeclaredSymbol(node);
        if (symbol.HasAttribute<StubAttribute>(_context.SemanticModel))
            return;

        base.VisitRecordDeclaration(node);
    }

    public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
    {
        var symbol = _context.SemanticModel.GetDeclaredSymbol(node);

        if (symbol != null && !HasAttributes(symbol))
        {
            var references = SymbolFinder.FindReferencesAsync(symbol, _context.Solution).Result;
            var s = references.SelectMany(w => w.Locations).Count();
            if (s == 0)
                _signatureDictionary.Add(symbol.ToUniqueSignature(), !_allows.Fields.Contains(symbol.ToUniqueSignature()));
        }


        base.VisitFieldDeclaration(node);
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        var symbol = _context.SemanticModel.GetDeclaredSymbol(node);
        if (symbol.HasAttribute<StubAttribute>(_context.SemanticModel))
            return;

        if (symbol != null && !HasAttributes(symbol))
        {
            var references = SymbolFinder.FindReferencesAsync(symbol, _context.Solution).Result;
            var s = references.SelectMany(w => w.Locations).Count();
            if (s == 0)
                _signatureDictionary.Add(symbol.ToUniqueSignature(), !_allows.Methods.Contains(symbol.ToUniqueSignature()));
        }

        base.VisitMethodDeclaration(node);
    }


    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        var symbol = _context.SemanticModel.GetDeclaredSymbol(node);
        if (symbol.HasAttribute<StubAttribute>(_context.SemanticModel))
            return;

        if (symbol != null && !HasAttributes(symbol))
        {
            var references = SymbolFinder.FindReferencesAsync(symbol, _context.Solution).Result;
            var s = references.SelectMany(w => w.Locations).Count();
            if (s == 0)
                _signatureDictionary.Add(symbol.ToUniqueSignature(), !_allows.Properties.Contains(symbol.ToUniqueSignature()));
        }

        base.VisitPropertyDeclaration(node);
    }


    public override void VisitParameter(ParameterSyntax node)
    {
        var symbol = _context.SemanticModel.GetDeclaredSymbol(node);
        if (symbol.HasAttribute<MarkAttribute>(_context.SemanticModel))
            return;

        if (symbol != null && !HasAttributes(symbol))
        {
            var references = SymbolFinder.FindReferencesAsync(symbol, _context.Solution).Result;
            var s = references.SelectMany(w => w.Locations).Count();
            if (s == 0)
                _signatureDictionary.Add(symbol.ToUniqueSignature(), true);
        }

        base.VisitParameter(node);
    }

    public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
    {
        base.VisitLocalFunctionStatement(node);
    }
}