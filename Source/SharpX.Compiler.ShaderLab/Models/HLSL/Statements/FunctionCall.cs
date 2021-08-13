using System;
using System.Collections.Generic;
using System.Linq;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Statements
{
    internal class FunctionCall : INestableStatement
    {
        private readonly List<IStatement> _statements;
        private string? _identifier;

        public FunctionCall()
        {
            _identifier = null;
            _statements = new List<IStatement>();
        }

        public void WriteTo(SourceBuilder sb)
        {
            if (string.IsNullOrWhiteSpace(_identifier))
                throw new InvalidOperationException($"missing {nameof(_identifier)}");

            if (!sb.IsIndented)
                sb.WriteSpanWithIndent(_identifier);
            else
                sb.WriteSpan(_identifier);
            sb.WriteSpan("(");

            foreach (var (statement, i) in _statements.Select((w, i) => (w, i)))
            {
                if (i > 0)
                    sb.WriteSpan(", ");
                statement.WriteTo(sb);
            }

            sb.WriteSpan(")");
        }

        public void AddSourcePart(INestableStatement statement)
        {
            _statements.Add(statement);
        }

        public void AddSourcePart(IStatement statement)
        {
            _statements.Add(statement);
        }

        public void AddIdentifier(string identifier)
        {
            _identifier = identifier;
        }
    }
}