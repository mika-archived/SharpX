namespace ShaderSharp.Library.Interfaces.Fragment
{
    public interface IFragmentShader<out TGlobals, in TInput, out TOutput> where TGlobals : IGlobals where TInput : IFragmentInput where TOutput : IFragmentOutput
    {
        protected TGlobals Globals { get; }

        TOutput FragmentMain(TInput i);
    }
}