using System.Collections.Generic;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Statements
{
    internal class Expression : INestableStatement
    {
        private readonly List<IStatement> _statements;
        private readonly bool _isIndented;

        public Expression(bool isIndented = false)
        {
            _isIndented = isIndented;
            _statements = new List<IStatement>();
        }

        public void WriteTo(SourceBuilder sb)
        {
            if (_isIndented)
                sb.WriteIndent();

            foreach (var statement in _statements)
                statement.WriteTo(sb);
        }

        public void AddSourcePart(INestableStatement statement)
        {
            _statements.Add(statement);
        }

        public void AddSourcePart(IStatement statement)
        {
            _statements.Add(statement);
        }

        public string ToSourceString()
        {
            var sb = new SourceBuilder();

            WriteTo(sb);

            return sb.ToSource();
        }

        public IReadOnlyList<IStatement> CastUp()
        {
            return _statements;
        }
    }
}