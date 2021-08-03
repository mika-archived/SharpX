namespace ShaderSharp.Library.Interfaces.Vertex
{
    public interface IVertexShader<in TInput, out TOutput> where TInput : IVertexInput where TOutput : IVertexOutput
    {
        TOutput VertexMain(TInput i);
    }
}