namespace ShaderSharp.Library.Interfaces.Fragment
{
    public interface IFragmentShader<in TInput, out TOutput> where TInput : IFragmentInput where TOutput : IFragmentOutput
    {
        TOutput FragmentMain(TInput i);
    }
}