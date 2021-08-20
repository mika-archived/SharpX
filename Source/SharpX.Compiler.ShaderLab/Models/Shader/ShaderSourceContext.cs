using System.Collections.Generic;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.Shader
{
    internal class ShaderSourceContext : ISourceContext
    {
        public string Name { get; set; }

        public List<ShaderProperty> Properties { get; } = new();

        public string ToSourceString()
        {
            var sb = new SourceBuilder();
            sb.WriteLineWithIndent($"Shader \"{Name}\" {{");
            sb.IncrementIndent();

            if (Properties.Count > 0)
            {
                sb.WriteLineWithIndent("Properties {");
                sb.IncrementIndent();

                foreach (var property in Properties)
                {
                    foreach (var attribute in property.InspectorAttributes)
                        sb.WriteLineWithIndent($"[{attribute}]");
                    sb.WriteLineWithIndent($"{property.Name} (\"{property.DisplayName}\", {property.ConvertedType}) = {property.Default}");
                }

                sb.DecrementIndent();
                sb.WriteLineWithIndent("}");
            }

            sb.DecrementIndent();
            sb.WriteLineWithIndent("}");
            return sb.ToSource();
        }
    }
}