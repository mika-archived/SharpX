using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Library.ShaderLab.Primitives
{
    [Component("sampler3D")]
    [External]
    public class Sampler3D
    {
        public static implicit operator Sampler3D(string _)
        {
            return default;
        }
    }
}