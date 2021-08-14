using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Library.ShaderLab.Interfaces
{
    [External]
    [Component("PointStream<&T>")]
    public interface IPointStream<in T> : IVertexStream<T> { }
}