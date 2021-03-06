using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using SharpX.CodeGen.ShaderLab.Models;

namespace SharpX.CodeGen.ShaderLab.Generators
{
    [Generator]
    internal class ShaderFunctionGenerator : ISourceGenerator
    {
        private const string AttributeFullyQualifiedTypedName = "SharpX.CodeGen.ShaderLab.Attributes.AutoGeneratedAttribute";

        private const string AttributeClass = @"
using System;
using System.Diagnostics;

namespace SharpX.CodeGen.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    [Conditional(""COMPILE_TIME_ONLY"")]
    public class AutoGeneratedAttribute : Attribute
    {
        public AutoGeneratedAttribute(string reference) { }
    }
}
";

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForPostInitialization(w => w.AddSource("AutoGeneratedAttribute", AttributeClass));
            context.RegisterForSyntaxNotifications(() => new SyntaxContextReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is not SyntaxContextReceiver receiver)
                return;

            context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.projectdir", out var projectDir);
            if (string.IsNullOrWhiteSpace(projectDir))
                return;

            var attributeSymbol = context.Compilation.GetTypeByMetadataName(AttributeFullyQualifiedTypedName);
            if (attributeSymbol == null)
                return;

            foreach (var @class in receiver.Classes)
            {
                var source = GenerateSourceFromReference(@class, attributeSymbol, projectDir!, context.AdditionalFiles);
                if (string.IsNullOrWhiteSpace(source))
                    continue;

                context.AddSource($"{@class.Name}.g.cs", SourceText.From(source!, Encoding.UTF8));
            }
        }

        private string? GenerateSourceFromReference(INamedTypeSymbol classSymbol, INamedTypeSymbol attributeSymbol, string projectDir, ImmutableArray<AdditionalText> additionalTexts)
        {
            var attr = classSymbol.GetAttributes().FirstOrDefault(w => w.AttributeClass != null && w.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default));
            if (attr == null)
                return null;

            var resources = attr.ConstructorArguments.FirstOrDefault();
            if (resources.IsNull || resources.Value is not string value)
                return null;

            var path = Path.Combine(projectDir, value);
            if (additionalTexts.Any(w => w.Path == path))
                return null;

            using var sr = new StreamReader(path);
            using var parser = new FunctionDefinitionParser(sr);
            parser.Parse();

            var sb = new StringBuilder();
            sb.Append("namespace ").AppendLine(parser.Namespace);
            sb.AppendLine("{");

            if (!string.IsNullOrWhiteSpace(parser.Include))
                sb.AppendLine($@"    [SharpX.Library.ShaderLab.Attributes.Include(""{parser.Include}"")]");

            sb.Append("    public static partial class ").AppendLine(parser.Class);
            sb.AppendLine("    {");

            foreach (var function in parser.Functions)
                sb.AppendLine(function.ToMethodDeclaration());

            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
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