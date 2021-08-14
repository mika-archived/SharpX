using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Statements
{
    internal class Assignment : IStatement
    {
        private readonly INestableStatement _leftStatement;
        private readonly string _operator;
        private readonly INestableStatement _rightStatement;

        public Assignment(string @operator, INestableStatement leftStatement, INestableStatement rightStatement)
        {
            _operator = @operator;
            _leftStatement = leftStatement;
            _rightStatement = rightStatement;
        }

        public void WriteTo(SourceBuilder sb)
        {
            _leftStatement.WriteTo(sb);
            sb.WriteSpan($" {_operator} ");
            _rightStatement.WriteTo(sb);
        }
    }
}