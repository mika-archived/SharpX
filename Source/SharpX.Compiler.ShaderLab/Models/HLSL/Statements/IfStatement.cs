using System;
using System.Collections.Generic;
using System.Linq;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Statements
{
    internal class IfStatement : INestableStatement
    {
        private readonly List<IStatement> _statements;
        private IStatement? _condition;

        public IfStatement()
        {
            _statements = new List<IStatement>();
        }

        public void WriteTo(SourceBuilder sb)
        {
            if (_condition == null)
                throw new ArgumentNullException();

            if (sb.IsIndented)
                sb.WriteSpan("if (");
            else
                sb.WriteSpanWithIndent("if (");

            _condition.WriteTo(sb);
            sb.WriteLine(")");

            var shouldIndent = _statements.FirstOrDefault() is not Block;
            if (shouldIndent)
                sb.IncrementIndent();

            foreach (var statement in _statements)
                statement.WriteTo(sb);

            if (shouldIndent)
                sb.DecrementIndent();
        }

        public void AddSourcePart(INestableStatement statement)
        {
            _statements.Add(statement);
        }

        public void AddSourcePart(IStatement statement)
        {
            _statements.Add(statement);
        }

        public void AddCondition(IStatement condition)
        {
            _condition = condition;
        }
    }
}