using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Statements
{
    public class EmptyStatement : IStatement
    {
        public void WriteTo(SourceBuilder sb)
        {
            sb.WriteNewLine();
        }
    }
}