using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Library.ShaderLab.Primitives
{
    [Component("sampler2D")]
    [External]
    public class Sampler2D
    {
        public static implicit operator Sampler2D(string _)
        {
            return default;
        }
    }
}