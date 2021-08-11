using SharpX.Compiler.Composition.Abstractions;

namespace SharpX.Compiler.ShaderLab.Models.HLSL
{
    // ReSharper disable once InconsistentNaming
    internal class HLSLFunctionDefinitionScope : ContextScope
    {
        public HLSLFunctionDefinitionScope(ContextScope scope) : base(scope) { }

        public override void VerifyCurrentIntegrity<T>()
        {
            VerifyParentIntegrity<DefaultRootScope>("struct declaration must be always in the top-level scope.");
        }
    }
}