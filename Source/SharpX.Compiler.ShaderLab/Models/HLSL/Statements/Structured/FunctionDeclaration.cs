using System.Collections.Generic;
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
        private readonly string? _returnsSemantics;
        private readonly List<IStatement> _statements;

        public FunctionDeclaration(string name, string returns, string? semantics = null)
        {
            _name = name;
            _returns = returns;
            _returnsSemantics = semantics;
            _arguments = new List<KeyValuePair<string, string>>();
            _attributes = new List<string>();
            _statements = new List<IStatement>();
        }

        public int Priority { get; set; } = 40000;

        public string Name => $"Function_{_name}_{_returns}";

        public void WriteTo(SourceBuilder sb)
        {
            foreach (var attribute in _attributes)
                sb.WriteLine($"[{attribute}]");

            sb.WriteSpan($"{_returns} {_name}(");
            sb.WriteSpan(string.Join(", ", _arguments.Select(w => $"{w.Key} {w.Value}")));
            sb.WriteSpan(")");

            if (string.IsNullOrWhiteSpace(_returnsSemantics))
                sb.WriteNewLine();
            else
                sb.WriteLine($" : {_returnsSemantics}");

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

        public void AddArgumentWithSemantics(string type, string name, string semantics)
        {
            _arguments.Add(new KeyValuePair<string, string>(type, $"{name} : {semantics}"));
        }

        public void AddAttributedArgument(string attribute, string type, string name, string? @default = null)
        {
            if (string.IsNullOrWhiteSpace(@default))
                _arguments.Add(new KeyValuePair<string, string>($"{attribute} {type}".Trim(), name));
            else
                _arguments.Add(new KeyValuePair<string, string>($"{attribute} {type}".Trim(), $"{name} = {@default}"));
        }

        public void AddAttributedArgumentWithSemantics(string attribute, string type, string name, string semantics)
        {
            _arguments.Add(new KeyValuePair<string, string>($"{attribute} {type}".Trim(), $"{name} : {semantics}"));
        }

        public void AddSourcePart(IStatement statement)
        {
            _statements.Add(statement);
        }
    }
}