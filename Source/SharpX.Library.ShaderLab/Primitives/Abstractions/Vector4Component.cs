using SharpX.CodeGen.ShaderLab.Attributes;

namespace SharpX.Library.ShaderLab.Primitives.Abstractions
{
    [Swizzle(4, "X", "Y", "Z", "W")]
    [Swizzle(4, "R", "G", "B", "A")]
    public partial class Vector4Component<T1, T2, T3, T4> { }
}