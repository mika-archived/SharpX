namespace ShaderSharp.Library.Interfaces.Vertex
{
    public interface IVertexShader<out TGlobals, in TInput, out TOutput> where TGlobals : IGlobals where TInput : IVertexInput where TOutput : IVertexOutput
    {
        TGlobals Globals { get; }

        TOutput VertexMain(TInput i);
    }
}