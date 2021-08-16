using System.Collections.Generic;
using System.Linq;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Statements
{
    internal class ElseStatement : INestableStatement
    {
        private readonly List<IStatement> _statements;

        public ElseStatement()
        {
            _statements = new List<IStatement>();
        }

        public void WriteTo(SourceBuilder sb)
        {
            var isIfElseStatement = _statements.First() is IfStatement;
            if (isIfElseStatement)
                sb.WriteSpanWithIndent("else ");
            else
                sb.WriteLineWithIndent("else");

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