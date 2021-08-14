using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Library.ShaderLab.Interfaces
{
    [External]
    public interface IVertexStream<in T>
    {
        void Append(T point);

        void RestartStrip();
    }
}