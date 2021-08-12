using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Statements
{
    internal class Span : IStatement
    {
        private readonly string _span;

        public Span(string span)
        {
            _span = span;
        }

        public void WriteTo(SourceBuilder sb)
        {
            sb.WriteSpan(_span);
        }
    }
}