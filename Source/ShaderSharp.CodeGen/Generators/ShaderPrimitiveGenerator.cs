using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using ShaderSharp.CodeGen.Templates;

namespace ShaderSharp.CodeGen.Generators
{
    [Generator]
    public class ShaderPrimitiveGenerator : ISourceGenerator
    {
        private const string AttributeFullyQualifiedTypedName = "ShaderSharp.Library.Attributes.Internal.PrimitiveAttribute";

        private const string AttributeClass = @"
using System;
using System.Diagnostics;

namespace ShaderSharp.Library.Attributes.Internal
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

            foreach (var @class in receiver.Classes)
            {
                var source = GenerateSourceFromTemplate(@class, attributeSymbol);
                context.AddSource($"{@class.Name}.Generated.cs", SourceText.From(source, Encoding.UTF8));
            }
        }

        private string GenerateSourceFromTemplate(INamedTypeSymbol classSymbol, INamedTypeSymbol attributeSymbol)
        {
            var attr = classSymbol.GetAttributes().FirstOrDefault(w => w.AttributeClass != null && w.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default));
            if (attr is not { ConstructorArguments: { Length: 3 } })
                return null;

            var name = attr.ConstructorArguments[0];
            var type = attr.ConstructorArguments[1];
            var tstr = attr.ConstructorArguments[2];

            if (name.IsNull || type.IsNull || tstr.IsNull)
                return null;

            var template = new ShaderPrimitiveTemplate
            {
                ClassName = classSymbol.Name,
                ComponentName = name.Value as string,
                CSharpPrimitive = (type.Value as INamedTypeSymbol)?.ToDisplayString(),
                Inheritance = GetInheritanceInterface(type.Value as INamedTypeSymbol, tstr.Value as string),
                Template = tstr.Value as string
            };

            return template.TransformText();
        }

        private string GetInheritanceInterface(INamedTypeSymbol type, string tstr)
        {
            switch (type.ToDisplayString())
            {
                case "bool" when tstr == "1":
                    return "IBoolComponent";

                case "float" when tstr == "1":
                    return "IFloatComponent";

                case "int" when tstr == "1":
                    return "IIntComponent";

                case "uint" when tstr == "1":
                    return "IUintComponent";

                case { } when int.TryParse(tstr, out _):
                    return $"Vector{tstr}Component<{GetShaderSharpPrimitiveFromCSharpPrimitive(type)}>";

                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        private string GetShaderSharpPrimitiveFromCSharpPrimitive(INamedTypeSymbol type)
        {
            switch (type.ToDisplayString())
            {
                case "bool":
                    return "SlBool";

                case "float":
                    return "SlFloat";

                case "int":
                    return "SlInt";

                case "uint":
                    return "SlUInt";

                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
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