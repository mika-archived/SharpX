using SharpX.Compiler.Composition.Abstractions;

namespace SharpX.Compiler.Udon.Models
{
    internal class UdonSourceContext : VerifiableSourceContext
    {
        private readonly UdonStructuredSourceBuilder _sb;

        public UdonSourceContext()
        {
            _sb = new UdonStructuredSourceBuilder();
        }

        public override string ToSourceString()
        {
            return _sb.ToSource();
        }
    }
}