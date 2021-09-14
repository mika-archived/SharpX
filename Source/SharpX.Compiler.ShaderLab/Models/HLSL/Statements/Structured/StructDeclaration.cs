using System.Collections.Generic;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Statements.Structured
{
    internal class StructDeclaration : IStructuredComponent
    {
        private readonly HashSet<ShaderLabFieldMember> _members;
        private readonly string _name;

        public StructDeclaration(string name)
        {
            _name = name;
            _members = new HashSet<ShaderLabFieldMember>();

            Name = $"Struct_{_name}";
        }

        public int Priority { get; set; } = 30000;

        public string Name { get; }

        public void WriteTo(SourceBuilder sb)
        {
            sb.WriteLine($"struct {_name} {{");
            sb.IncrementIndent();

            foreach (var member in _members)
                if (member.IsMacro)
                    sb.WriteLineWithIndent(member.Name);
                else if (string.IsNullOrWhiteSpace(member.Semantics))
                    sb.WriteLineWithIndent(string.IsNullOrWhiteSpace(member.Modifier) ? $"{member.Type} {member.Name};" : $"{member.Modifier} {member.Type} {member.Name};");
                else
                    sb.WriteLineWithIndent(string.IsNullOrWhiteSpace(member.Modifier) ? $"{member.Type} {member.Name} : {member.Semantics};" : $"{member.Modifier} {member.Type} {member.Name} : {member.Semantics};");

            sb.DecrementIndent();
            sb.WriteLine("};");
            sb.WriteNewLine();
        }

        public void AddMember(string str)
        {
            _members.Add(new ShaderLabFieldMember(null, str, null, true, null));
        }

        public void AddMember(string type, string name, string? semantics)
        {
            _members.Add(new ShaderLabFieldMember(type, name, semantics, false, null));
        }

        public void AddMember(string type, string name, string? semantics, string? modifier)
        {
            _members.Add(new ShaderLabFieldMember(type, name, semantics, false, modifier));
        }

        private record ShaderLabFieldMember(string? Type, string Name, string? Semantics, bool IsMacro, string? Modifier);
    }
}