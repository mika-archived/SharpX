namespace ShaderSharp.Compiler.Models.Context
{
    public class RootContextScope : ContextScope
    {
        public RootContextScope() : base(null) { }

        public override void VerifyIntegrity()
        {
            // Nothing to do
        }
    }
}