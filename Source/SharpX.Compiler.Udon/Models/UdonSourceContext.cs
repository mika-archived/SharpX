using SharpX.Compiler.Composition.Abstractions;

namespace SharpX.Compiler.Udon.Models
{
    internal class UdonSourceContext : VerifiableSourceContext
    {
        public UasmBuilder UasmBuilder { get; }

        public UdonSourceContext()
        {
            UasmBuilder = new UasmBuilder();
        }


        public override string ToSourceString()
        {
            return UasmBuilder.ToAssemblyString();
        }
    }
}