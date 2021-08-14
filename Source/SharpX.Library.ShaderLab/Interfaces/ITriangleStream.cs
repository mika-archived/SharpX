using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Library.ShaderLab.Interfaces
{
    [External]
    [Component("TriangleStream<&T>")]
    public interface ITriangleStream<in T> : IVertexStream<T> { }
}