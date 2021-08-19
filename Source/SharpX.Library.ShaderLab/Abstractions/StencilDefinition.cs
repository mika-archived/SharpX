using SharpX.Library.ShaderLab.Interfaces;

#nullable enable

namespace SharpX.Library.ShaderLab.Abstractions
{
    public class StencilDefinition : IShader
    {
        public string? Ref { get; protected set; }

        public string? ReadMask { get; protected set; }

        public string? WriteMask { get; protected set; }

        public string? Compare { get; protected set; }

        public string? Pass { get; protected set; }

        public string? Fail { get; protected set; }

        public string? ZFail { get; protected set; }

        protected StencilDefinition() { }
    }
}