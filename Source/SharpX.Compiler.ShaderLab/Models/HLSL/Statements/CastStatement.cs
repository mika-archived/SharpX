using System;
using System.Collections.Generic;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Statements
{
    internal class CastStatement : INestableStatement
    {
        private readonly List<IStatement> _statements;
        private readonly string _t;

        public CastStatement(string t)
        {
            _t = t;
            _statements = new List<IStatement>();
        }

        public void WriteTo(SourceBuilder sb)
        {
            sb.WriteSpan($"({_t}) ");

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