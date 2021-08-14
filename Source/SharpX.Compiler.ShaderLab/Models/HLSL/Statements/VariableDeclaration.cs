using System.Collections.Generic;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Statements
{
    internal class VariableDeclaration : INestableStatement
    {
        private readonly string _identifier;
        private readonly List<IStatement> _initializer;
        private readonly string _type;

        public VariableDeclaration(string type, string identifier)
        {
            _type = type;
            _identifier = identifier;
            _initializer = new List<IStatement>();
        }

        public void WriteTo(SourceBuilder sb)
        {
            sb.WriteSpan($"{_type} {_identifier}");

            if (_initializer.Count <= 0)
                return;

            sb.WriteSpan(" = ");
            foreach (var statement in _initializer)
                statement.WriteTo(sb);
        }

        public void AddSourcePart(INestableStatement statement)
        {
            _initializer.Add(statement);
        }

        public void AddSourcePart(IStatement statement)
        {
            _initializer.Add(statement);
        }
    }
}