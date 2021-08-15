using SharpX.CodeGen.ShaderLab.Attributes;

namespace SharpX.Library.ShaderLab.Primitives
{
    [Primitive("int", typeof(int), "1")]
    public sealed partial class SlInt
    {
        public static implicit operator SlInt(SlUint a)
        {
            return default;
        }
    }
}