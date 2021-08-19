using SharpX.Library.ShaderLab.Abstractions;

namespace SharpX.Examples.ShaderLab.Shader
{
    internal class Stencil : StencilDefinition
    {
        public Stencil()
        {
            Ref = "[_StencilRef]";
            Compare = "[_StencilCompare]";
            Pass = "[_StencilPass]";
        }
    }
}