using System.Collections.Generic;
using System.Linq;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Statements
{
    internal class MemberAccess : INestableStatement
    {
        private readonly List<IStatement> _statements;

        public MemberAccess()
        {
            _statements = new List<IStatement>();
        }

        public void WriteTo(SourceBuilder sb)
        {
            foreach (var (statement, i) in _statements.Select((w, i) => (w, i)))
            {
                if (i > 0)
                    sb.WriteSpan(".");
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