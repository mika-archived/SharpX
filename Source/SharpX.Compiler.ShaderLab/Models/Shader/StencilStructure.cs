using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.Shader
{
    internal class StencilStructure : IStatement
    {
        public string? Ref { get; set; }

        public string? ReadMask { get; set; }

        public string? WriteMask { get; set; }

        public string? Compare { get; set; }

        public string? Pass { get; set; }

        public string? Fail { get; set; }

        public string? ZFail { get; set; }

        public void WriteTo(SourceBuilder sb)
        {
            sb.WriteLineWithIndent("Stencil {");
            sb.IncrementIndent();

            if (!string.IsNullOrWhiteSpace(Ref))
                sb.WriteLineWithIndent($"Ref {Ref}");
            if (!string.IsNullOrWhiteSpace(ReadMask))
                sb.WriteLineWithIndent($"ReadMask {ReadMask}");
            if (!string.IsNullOrWhiteSpace(WriteMask))
                sb.WriteLineWithIndent($"WriteMask {WriteMask}");
            if (!string.IsNullOrWhiteSpace(Compare))
                sb.WriteLineWithIndent($"Comp {Compare}");
            if (!string.IsNullOrWhiteSpace(Pass))
                sb.WriteLineWithIndent($"Pass {Pass}");
            if (!string.IsNullOrWhiteSpace(Fail))
                sb.WriteLineWithIndent($"Fail {Fail}");
            if (!string.IsNullOrWhiteSpace(ZFail))
                sb.WriteLineWithIndent($"ZFail {ZFail}");

            sb.DecrementIndent();
            sb.WriteLineWithIndent("}");
        }
    }
}