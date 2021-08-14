using System.Collections.Generic;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Statements
{
    internal class Expression : INestableStatement
    {
        private readonly List<IStatement> _statements;

        public Expression()
        {
            _statements = new List<IStatement>();
        }

        public void WriteTo(SourceBuilder sb)
        {
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
    }
}