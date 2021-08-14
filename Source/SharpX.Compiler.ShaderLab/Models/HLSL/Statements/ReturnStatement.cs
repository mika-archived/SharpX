using System.Collections.Generic;
using System.Linq;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Statements
{
    public class ReturnStatement : INestableStatement
    {
        private readonly List<IStatement> _statements;

        public ReturnStatement()
        {
            _statements = new List<IStatement>();
        }

        public ReturnStatement(params IStatement[] statements)
        {
            _statements = statements.ToList();
        }

        public void WriteTo(SourceBuilder sb)
        {
            if (_statements.Count == 0)
            {
                sb.WriteSpan("return");
            }
            else
            {
                sb.WriteSpan("return ");
                foreach (var statement in _statements)
                    statement.WriteTo(sb);
            }
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