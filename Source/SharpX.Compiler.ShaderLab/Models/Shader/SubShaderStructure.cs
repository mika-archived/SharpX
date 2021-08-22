using System.Collections.Generic;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.Shader
{
    internal class SubShaderStructure : IStatement
    {
        public int? Lod { get; set; }

        public Dictionary<string, string> Tags { get; } = new();

        public string? GrabPass { get; set; }

        public List<ShaderPassStructure> Pass { get; } = new();

        public void WriteTo(SourceBuilder sb)
        {
            if (Lod.HasValue)
                sb.WriteLineWithIndent($"LOD {Lod.Value}");


            if (Tags.Keys.Count > 0)
            {
                sb.WriteLineWithIndent("Tags {");
                sb.IncrementIndent();

                foreach (var tag in Tags)
                    sb.WriteLineWithIndent($"\"{tag.Key}\" = \"{tag.Value}\"");

                sb.DecrementIndent();
                sb.WriteLineWithIndent("}");
            }


            // TODO: Ordering
            if (GrabPass != null)
                sb.WriteLineWithIndent(string.IsNullOrWhiteSpace(GrabPass) ? "GrabPass {}" : $"GrabPass {{ \"{GrabPass}\" }}");

            foreach (var pass in Pass) 
                pass.WriteTo(sb);
        }
    }
}