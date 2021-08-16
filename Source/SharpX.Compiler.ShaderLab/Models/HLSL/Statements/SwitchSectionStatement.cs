using System.Collections.Generic;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Statements
{
    internal class SwitchSectionStatement : INestableStatement
    {
        private readonly List<IStatement> _conditions;
        private readonly List<IStatement> _statements;

        public SwitchSectionStatement()
        {
            _conditions = new List<IStatement>();
            _statements = new List<IStatement>();
        }

        public void WriteTo(SourceBuilder sb)
        {
            if (_conditions.Count > 0)
                foreach (var condition in _conditions)
                {
                    sb.WriteSpanWithIndent("case ");
                    condition.WriteTo(sb);
                    sb.WriteLine(":");
                }
            else
                sb.WriteLineWithIndent("default:");

            sb.IncrementIndent();

            foreach (var statement in _statements)
                statement.WriteTo(sb);

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

        public void AddCondition(IStatement statement)
        {
            _conditions.Add(statement);
        }
    }
}