using System.Collections.Generic;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.Shader
{
    internal class ShaderPassStructure : IStatement
    {
        public string? AlphaToMask { get; set; }

        public string? Blend { get; set; }

        public (string, string?)? BlendOp { get; set; }

        public string? ColorMask { get; set; }

        public string? Culling { get; set; }

        public (float, float)? Offset { get; set; }

        public string? Name { get; set; }

        public Dictionary<string, string> Pragmas { get; } = new();

        public List<string> ShaderIncludes { get; } = new();

        public Dictionary<string, string> Tags { get; } = new();

        public StencilStructure? Stencil { get; set; }

        public string? ZTest { get; set; }

        public string? ZWrite { get; set; }

        public string? GrabPass { get; set; }

        public void WriteTo(SourceBuilder sb)
        {
            if (GrabPass != null)
            {
                sb.WriteLineWithIndent("GrabPass {");
                if (!string.IsNullOrWhiteSpace(GrabPass))
                {
                    sb.IncrementIndent();
                    sb.WriteLineWithIndent($"\"{GrabPass}\"");
                    sb.DecrementIndent();
                }
                sb.WriteLineWithIndent("}");

                return;
            }

            sb.WriteLineWithIndent("Pass {");
            sb.IncrementIndent();

            if (!string.IsNullOrWhiteSpace(AlphaToMask))
                sb.WriteLineWithIndent($"AlphaMask {AlphaToMask}");
            if (!string.IsNullOrWhiteSpace(Blend))
                sb.WriteLineWithIndent($"Blend {Blend}");
            if (BlendOp != null)
                sb.WriteLineWithIndent(string.IsNullOrWhiteSpace(BlendOp.Value.Item2) ? $"BlendOp {BlendOp.Value.Item1}" : $"BlendOp {BlendOp.Value.Item1} {BlendOp.Value.Item2}");
            if (!string.IsNullOrWhiteSpace(ColorMask))
                sb.WriteLineWithIndent($"ColorMask {ColorMask}");
            if (!string.IsNullOrWhiteSpace(Culling))
                sb.WriteLineWithIndent($"Cull {Culling}");
            if (Offset != null)
                sb.WriteLineWithIndent($"Offset {Offset.Value.Item1} {Offset.Value.Item2}");
            if (!string.IsNullOrWhiteSpace(Name))
                sb.WriteLineWithIndent($"Name \"{Name}\"");
            if (!string.IsNullOrWhiteSpace(ZTest))
                sb.WriteLineWithIndent($"ZTest {ZTest}");
            if (!string.IsNullOrWhiteSpace(ZWrite))
                sb.WriteLineWithIndent($"ZWrite {ZWrite}");
            if (Stencil != null)
                Stencil.WriteTo(sb);

            if (Tags.Keys.Count > 0)
            {
                sb.WriteLineWithIndent("Tags {");
                sb.IncrementIndent();

                foreach (var tag in Tags)
                    sb.WriteLineWithIndent($"\"{tag.Key}\" = \"{tag.Value}\"");

                sb.DecrementIndent();
                sb.WriteLineWithIndent("}");
            }

            sb.WriteLineWithIndent("CGPROGRAM");

            foreach (var pragma in Pragmas)
                sb.WriteLineWithIndent($"#pragma {pragma.Key} {pragma.Value}".Trim());
            foreach (var include in ShaderIncludes) 
                sb.WriteLineWithIndent($"#include \"{include}\"");

            sb.WriteLineWithIndent("ENDCG");

            sb.DecrementIndent();
            sb.WriteLineWithIndent("}");
        }
    }
}