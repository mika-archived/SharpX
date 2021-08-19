using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Library.ShaderLab.Primitives
{
    [Component("sampler1D")]
    [External]
    public class Sampler1D
    {
        public static implicit operator Sampler1D(string _)
        {
            return default;
        }
    }
}