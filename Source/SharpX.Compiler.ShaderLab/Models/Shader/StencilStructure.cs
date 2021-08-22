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

        public void WriteTo(SourceBuilder sb) { }
    }
}