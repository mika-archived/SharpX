namespace ShaderSharp.Library.Interfaces.Geometry
{
    public interface IStreamOutputObject<in TOutput> where TOutput : IGeometryOutput
    {
        void Append(TOutput o);

        void RestartStrip();
    }
}