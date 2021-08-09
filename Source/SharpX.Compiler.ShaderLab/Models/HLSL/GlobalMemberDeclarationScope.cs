using SharpX.Compiler.Composition.Abstractions;

namespace SharpX.Compiler.ShaderLab.Models.HLSL
{
    internal class GlobalMemberDeclarationScope : DisposableContextScope
    {
        public GlobalMemberDeclarationScope(VerifiableSourceContext context, ContextScope scope) : base(context, scope) { }

        public override void VerifyIntegrity()
        {
            VerifyParentIntegrity<DefaultRootScope>();
        }
    }
}