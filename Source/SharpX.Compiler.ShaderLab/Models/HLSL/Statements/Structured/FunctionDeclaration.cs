using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Statements.Structured
{
    internal class FunctionDeclaration : IStructuredComponent
    {
        private readonly List<KeyValuePair<string, string>> _arguments;
        private readonly List<string> _attributes;
        private readonly string _name;
        private readonly string _returns;
        private readonly List<IStatement> _statements;

        public FunctionDeclaration(string name, string returns)
        {
            _name = name;
            _returns = returns;
            _arguments = new List<KeyValuePair<string, string>>();
            _attributes = new List<string>();
            _statements = new List<IStatement>();
        }

        public int Priority { get; set; } = 40000;

        public string Name => $"Function_{_name}_{_returns}__{string.Join("_", _arguments)}";

        public void WriteTo(SourceBuilder sb)
        {
            foreach (var attribute in _attributes)
                sb.WriteLine(attribute);

            sb.WriteSpan($"{_returns} {_name}(");
            sb.WriteSpan(string.Join(", ", _arguments.Select(w => $"{w.Key} {w.Value}")));
            sb.WriteLine(")");

            foreach (var statement in _statements)
                statement.WriteTo(sb);

            sb.WriteNewLine();
        }

        public void AddAttribute(string attribute)
        {
            _attributes.Add(attribute);
        }

        public void AddArgument(string type, string name, string? @default = null)
        {
            if (string.IsNullOrWhiteSpace(@default))
                _arguments.Add(new KeyValuePair<string, string>(type, name));
            else
                _arguments.Add(new KeyValuePair<string, string>(type, $"{name} = {@default}"));
        }

        public void AddAttributedArgument(string attribute, string type, string name, string? @default = null)
        {
            if (string.IsNullOrWhiteSpace(@default))
                _arguments.Add(new KeyValuePair<string, string>($"{attribute} {type}", name));
            else
                _arguments.Add(new KeyValuePair<string, string>($"{attribute} {type}", $"{name} = {@default}"));
        }

        public void AddSourcePart(IStatement statement)
        {
            _statements.Add(statement);
        }

        public bool PopLastSourcePartIfAvailable([NotNullWhen(true)] out INestableStatement? s)
        {
            var statement = _statements.Last();
            if (statement is INestableStatement nestable)
            {
                s = nestable;
                return true;
            }

            s = default;
            return false;
        }

        public bool PopLastSourcePartIfAvailable<T>([NotNullWhen(true)] out T? s) where T : INestableStatement
        {
            var statement = _statements.Last();
            if (statement is T nestable)
            {
                s = nestable;
                return true;
            }

            s = default;
            return false;
        }
    }
}