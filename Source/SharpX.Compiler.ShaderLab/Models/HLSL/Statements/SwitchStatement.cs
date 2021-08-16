using System;
using System.Collections.Generic;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Statements
{
    internal class SwitchStatement : INestableStatement
    {
        private readonly INestableStatement _condition;
        private readonly List<IStatement> _statements;

        public SwitchStatement(INestableStatement condition)
        {
            _condition = condition;
            _statements = new List<IStatement>();
        }

        public void WriteTo(SourceBuilder sb)
        {
            sb.WriteSpanWithIndent("switch (");
            _condition.WriteTo(sb);
            sb.WriteLine(")");

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