using System.Collections.Generic;
using System.Linq;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Statements
{
    internal class ForStatement : INestableStatement
    {
        private readonly IStatement _condition;
        private readonly IStatement _incrementor;
        private readonly IStatement _initializer;
        private readonly List<IStatement> _statements;

        public ForStatement(Expression initializer, IStatement condition, IStatement incrementor)
        {
            _initializer = initializer.CastUp().First() is Statement s ? s.IntoExpression() : initializer; // initializer is nested......
            _condition = condition;
            _incrementor = incrementor;
            _statements = new List<IStatement>();
        }

        public void WriteTo(SourceBuilder sb)
        {
            sb.WriteSpanWithIndent("for (");
            _initializer.WriteTo(sb);
            sb.WriteSpan("; ");
            _condition.WriteTo(sb);
            sb.WriteSpan("; ");
            _incrementor.WriteTo(sb);
            sb.WriteSpan(")");
            sb.WriteNewLine();

            var shouldIndent = _statements.First() is not Block;

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
    }
}