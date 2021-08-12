using System.Collections.Generic;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Statements
{
    internal class Block : INestableStatement
    {
        private readonly List<IStatement> _statements;

        public Block()
        {
            _statements = new List<IStatement>();
        }

        public void WriteTo(SourceBuilder sb)
        {
            sb.WriteLineWithIndent("{");
            sb.IncrementIndent();

            foreach (var statement in _statements)
                statement.WriteTo(sb);

            sb.DecrementIndent();
            sb.WriteLineWithIndent("}");
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