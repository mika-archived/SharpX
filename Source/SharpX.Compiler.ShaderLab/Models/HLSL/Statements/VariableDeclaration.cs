using System.Collections.Generic;
using System.Linq;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Statements
{
    internal class VariableDeclaration : INestableStatement
    {
        private readonly string _identifier;
        private readonly List<IStatement> _initializer;
        private readonly string _type;
        private int? _arrayCount;

        public VariableDeclaration(string type, string identifier)
        {
            _type = type;
            _identifier = identifier;
            _initializer = new List<IStatement>();
            _arrayCount = null;
        }

        public VariableDeclaration(string type, string identifier, params IStatement[] initializer)
        {
            _type = type;
            _identifier = identifier;
            _initializer = initializer.ToList();
        }

        public void WriteTo(SourceBuilder sb)
        {
            sb.WriteSpan($"{_type} {_identifier}");

            if (_arrayCount.HasValue)
                sb.WriteSpan($"[{_arrayCount.Value}]");

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

        public void AddArrayCount(int count)
        {
            _arrayCount = count;
        }
    }
}