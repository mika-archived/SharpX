using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using SharpX.CodeGen.ShaderLab.Templates;

namespace SharpX.CodeGen.ShaderLab.Generators
{
    [Generator]
    public class ShaderPrimitiveGenerator : ISourceGenerator
    {
        private const string AttributeFullyQualifiedTypedName = "SharpX.CodeGen.ShaderLab.Attributes.PrimitiveAttribute";

        private const string AttributeClass = @"
using System;
using System.Diagnostics;

namespace SharpX.CodeGen.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    [Conditional(""COMPILE_TIME_ONLY"")]
    public class PrimitiveAttribute : Attribute
    {
        public PrimitiveAttribute(string name, Type t, string template) { }
    }
}
";

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForPostInitialization(w => w.AddSource("PrimitiveAttribute", AttributeClass));
            context.RegisterForSyntaxNotifications(() => new SyntaxContextReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is not SyntaxContextReceiver receiver)
                return;

            var attributeSymbol = context.Compilation.GetTypeByMetadataName(AttributeFullyQualifiedTypedName);
            if (attributeSymbol == null)
                return;

            foreach (var @class in receiver.Classes)
            {
                var source = GenerateSourceFromTemplate(@class, attributeSymbol);
                if (string.IsNullOrWhiteSpace(source))
                    continue;

                context.AddSource($"{@class.Name}.g.cs", SourceText.From(source!, Encoding.UTF8));
            }
        }

        private string? GenerateSourceFromTemplate(INamedTypeSymbol classSymbol, INamedTypeSymbol attributeSymbol)
        {
            var attr = classSymbol.GetAttributes().FirstOrDefault(w => w.AttributeClass != null && w.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default));
            if (attr is not { ConstructorArguments: { Length: 3 } })
                return null;

            var name = attr.ConstructorArguments[0];
            var type = attr.ConstructorArguments[1];
            var tstr = attr.ConstructorArguments[2];

            if (name.IsNull || type.IsNull || tstr.IsNull)
                return null;

            var component = (string) name.Value!;
            var primitive = ((INamedTypeSymbol) type.Value!)?.ToDisplayString() ?? "";
            var ttemplate = (string) tstr.Value!;

            var template = new ShaderPrimitiveTemplate(component, classSymbol.Name, ttemplate, primitive);
            if (!string.IsNullOrWhiteSpace(GetInheritanceInterface(primitive, ttemplate, component)))
                template.Inheritance = GetInheritanceInterface(primitive, ttemplate, component);

            return template.TransformText();
        }

        private string? GetInheritanceInterface(string type, string tstr, string component)
        {
            switch (type)
            {
                case { } when int.TryParse(tstr, out var i) && i >= 2:
                    return $"Vector{tstr}Component<{GetShaderSharpPrimitiveFromCSharpPrimitive(component.Substring(0, component.Length - 1))}>";

                default:
                    return null;
            }
        }

        private string GetShaderSharpPrimitiveFromCSharpPrimitive(string type)
        {
            var component = type switch
            {
                "bool" => "SlBool",
                "float" => "SlFloat",
                "int" => "SlInt",
                "uint" => "SlUint",
                "fixed" => "SlFixed",
                "half" => "SlHalf",
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };

            return string.Join(", ", Enumerable.Repeat(component, 4).Select((w, j) => j + 1 == 1 ? w : $"{w}{j + 1}"));
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