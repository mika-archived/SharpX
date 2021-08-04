using ShaderSharp.Library.Attributes.Internal;
using ShaderSharp.Library.Primitives.Interfaces;

namespace ShaderSharp.Library.Primitives.Abstractions
{
    [Swizzle(4, "X", "Y", "Z")]
    [Swizzle(4, "R", "G", "B")]
    public abstract partial class Vector3Component<T1, T2, T3, T4> : IVectorComponent<T1> { }
}