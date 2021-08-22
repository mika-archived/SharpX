using System.Collections.Generic;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.Shader
{
    internal class ShaderSourceContext : ISourceContext
    {
        public string Name { get; set; } = "SharpX.Placeholder";

        public List<ShaderProperty> Properties { get; } = new();

        public List<SubShaderStructure> SubShaders { get;  } = new();

        public string? CustomEditor { get; set; }

        public string? Fallback { get; set; }

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

            foreach (var shader in SubShaders)
            {
                sb.WriteLineWithIndent("SubShader {");
                sb.IncrementIndent();

                shader.WriteTo(sb);

                sb.DecrementIndent();
                sb.WriteLineWithIndent("}");
            }

            if (!string.IsNullOrWhiteSpace(CustomEditor))
                sb.WriteLineWithIndent($"CustomEditor \"{CustomEditor}\"");
            if (!string.IsNullOrWhiteSpace(Fallback))
                sb.WriteLineWithIndent($"Fallback \"{Fallback}\"");

            sb.DecrementIndent();
            sb.WriteLineWithIndent("}");
            return sb.ToSource();
        }
    }
}