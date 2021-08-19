using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Library.ShaderLab.Primitives
{
    // ReSharper disable once InconsistentNaming
    [Component("samplerCUBE")]
    [External]
    public class SamplerCUBE
    {
        public static implicit operator SamplerCUBE(string _)
        {
            return default;
        }
    }
}