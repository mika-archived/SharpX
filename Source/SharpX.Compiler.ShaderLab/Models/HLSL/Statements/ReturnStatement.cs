using System.Collections.Generic;

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

        public void WriteTo(SourceBuilder sb)
        {
            if (_statements.Count == 0)
            {
                sb.WriteSpanWithIndent("return;");
            }
            else
            {
                sb.WriteSpanWithIndent("return ");
                foreach (var statement in _statements)
                    statement.WriteTo(sb);
                sb.WriteSpan(";");
            }

            sb.WriteNewLine();
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