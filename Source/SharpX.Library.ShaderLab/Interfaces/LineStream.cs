using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Library.ShaderLab.Interfaces
{
    [External]
    [Component("LineStream<&T>")]
    public interface ILineStream<in T> : IVertexStream<T> { }
}