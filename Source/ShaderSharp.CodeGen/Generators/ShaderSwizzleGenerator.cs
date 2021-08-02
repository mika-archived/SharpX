using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using ShaderSharp.CodeGen.Templates;

namespace ShaderSharp.CodeGen.Generators
{
    [Generator]
    public class ShaderSwizzleGenerator : ISourceGenerator
    {
        private const string AttributeFullyQualifiedTypedName = "ShaderSharp.Library.Attributes.Internal.SwizzleAttribute";

        private const string AttributeClass = @"
using System;
using System.Diagnostics;

namespace ShaderSharp.Library.Attributes.Internal
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = true)]
    [Conditional(""COMPILE_TIME_ONLY"")]
    public class SwizzleAttribute : Attribute
    {
        public SwizzleAttribute(int elements, params string[] attributes) { }
    }
}
";

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForPostInitialization(w => w.AddSource("SwizzleAttribute", AttributeClass));
            context.RegisterForSyntaxNotifications(() => new SyntaxContextReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is not SyntaxContextReceiver receiver)
                return;

            var attributeSymbol = context.Compilation.GetTypeByMetadataName(AttributeFullyQualifiedTypedName);

            foreach (var @class in receiver.Classes)
            {
                var sources = GenerateSourceFromTemplate(@class, attributeSymbol);
                foreach (var (source, i) in sources.Select((w, i) => (w, i)))
                    context.AddSource($"{@class.Name}.Generated{i}.cs", SourceText.From(source, Encoding.UTF8));
            }
        }

        private List<string> GenerateSourceFromTemplate(INamedTypeSymbol classSymbol, INamedTypeSymbol attributeSymbol)
        {
            var sources = new List<string>();
            var attributes = classSymbol.GetAttributes().Where(w => w.AttributeClass != null && w.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default)).ToArray();
            foreach (var attribute in attributes)
            {
                if (attribute.ConstructorArguments.Length != 2)
                    continue;

                try
                {
                    var length = (int) attribute.ConstructorArguments[0].Value!;
                    var components = attribute.ConstructorArguments[1].Values.Select(w => w.Value as string);

                    var template = new ShaderSwizzleTemplate
                    {
                        ClassName = classSymbol.ToDisplayString().Substring(classSymbol.ToDisplayString().LastIndexOf(".", StringComparison.Ordinal) + 1),
                        MaxLength = length,
                        Components = components.ToArray()
                    };

                    sources.Add(template.TransformText());
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }
            }

            return sources;
        }

        private class SyntaxContextReceiver : ISyntaxContextReceiver
        {
            public List<INamedTypeSymbol> Classes { get; } = new();

            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                if (context.Node is ClassDeclarationSyntax { AttributeLists: { Count: > 0 } } @class)
                    if (context.SemanticModel.GetDeclaredSymbol(@class) is INamedTypeSymbol symbol && symbol.GetAttributes().Any(w => w.AttributeClass?.ToDisplayString() == AttributeFullyQualifiedTypedName))
                        Classes.Add(symbol);
            }
        }
    }
}