using SharpX.Compiler.Composition.Abstractions;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.ScopeVerifiers
{
    // ReSharper disable once InconsistentNaming
    internal class HLSLStructDefinitionScope : ContextScope
    {
        public HLSLStructDefinitionScope(ContextScope scope) : base(scope) { }

        public override void VerifyIntegrity()
        {
            VerifyParentIntegrity<DefaultRootScope>("struct declaration must be always in the top-level scope.");
        }
    }
}