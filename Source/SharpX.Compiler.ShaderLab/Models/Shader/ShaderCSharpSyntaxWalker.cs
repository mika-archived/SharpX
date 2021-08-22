using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Loader;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CSharp.RuntimeBinder;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;
using SharpX.Compiler.Extensions;
using SharpX.Library.ShaderLab.Abstractions;
using SharpX.Library.ShaderLab.Attributes;
using SharpX.Library.ShaderLab.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.Shader
{
    public class ShaderCSharpSyntaxWalker : CSharpSyntaxWalker
    {
        private readonly ILanguageSyntaxWalkerContext _context;
        private readonly AssemblyLoadContext _loadContext;

        public ShaderCSharpSyntaxWalker(ILanguageSyntaxWalkerContext context, AssemblyLoadContext loadContext) : base(SyntaxWalkerDepth.Token)
        {
            _context = context;
            _loadContext = loadContext;
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (!node.HasInherit<IShader>(_context.SemanticModel))
                return;

            if (!node.HasInherit<ShaderLabDefinition>(_context.SemanticModel))
                return;

            if (!node.HasAttribute<ExportAttribute>(_context.SemanticModel))
                return;

            var symbol = _context.SemanticModel.GetDeclaredSymbol(node);
            if (symbol == null)
                return;

            var qualifiedName = symbol.ToDisplayString();

            using var stream = new MemoryStream();
            var emit = _context.SemanticModel.Compilation.Emit(stream);
            if (!emit.Success)
            {
                _context.Errors.Add(new DefaultError(node, $"Failed to compile definitions: \n\t{emit.Diagnostics.First().GetMessage()}"));
                return;
            }

            stream.Seek(0, SeekOrigin.Begin);

            var asm = _loadContext.LoadFromStream(stream);
            var t = asm.GetType(qualifiedName);
            if (t == null)
            {
                _context.Errors.Add(new DefaultError(node, $"Failed to find a type for {qualifiedName} in built assembly"));
                return;
            }

            var instance = Activator.CreateInstance(t) as dynamic; // it is a ShaderLabDefinition, but reference to "other" assembly.
            if (instance == null)
            {
                _context.Errors.Add(new DefaultError(node, $"Failed to construct {qualifiedName}, it does not have a default (empty arguments) constructor"));
                return;
            }

            var attr = node.GetAttribute<ExportAttribute>(_context.SemanticModel)!;
            _context.CreateOrGetContext<IShader>(attr.Source);

            try
            {
                var builder = new ShaderBuilder(_context.SourceContext.OfType<ShaderSourceContext>()!, _context.SemanticModel, instance);
                builder.Build();
            }
            catch (RuntimeBinderException e)
            {
                _context.Errors.Add(new DefaultError(node, e.Message));
            }
            catch (Exception e) when (Debugger.IsAttached)
            {
                Debug.WriteLine(e.Message);
            }

            _context.CloseContext();
        }
    }
}