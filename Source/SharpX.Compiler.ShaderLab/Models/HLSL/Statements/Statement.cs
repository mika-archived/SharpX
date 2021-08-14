using System.Collections.Generic;
using System.Linq;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Statements
{
    internal class Statement : INestableStatement
    {
        private readonly List<IStatement> _statements;

        public Statement()
        {
            _statements = new List<IStatement>();
        }

        public Statement(params IStatement[] statements)
        {
            _statements = statements.ToList();
        }

        public void WriteTo(SourceBuilder sb)
        {
            if (!sb.IsIndented)
                sb.WriteIndent();

            foreach (var statement in _statements)
                statement.WriteTo(sb);

            sb.WriteSpan(";");
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