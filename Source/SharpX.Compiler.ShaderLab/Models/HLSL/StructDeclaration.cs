using System.Collections.Generic;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL
{
    internal class StructDeclaration : IStructuredComponent
    {
        private readonly HashSet<ShaderLabFieldMember> _members;
        private readonly string _name;

        public StructDeclaration(string name)
        {
            _name = name;
            _members = new HashSet<ShaderLabFieldMember>();
        }

        public int Priority { get; set; } = 30000;

        public void WriteTo(SourceBuilder sb)
        {
            sb.WriteLine($"struct {_name} {{");
            sb.IncrementIndent();

            foreach (var member in _members)
                if (member.IsMacro)
                    sb.WriteLineWithIndent(member.Name);
                else if (string.IsNullOrWhiteSpace(member.Semantics))
                    sb.WriteLineWithIndent($"{member.Type} {member.Name};");
                else
                    sb.WriteLineWithIndent($"{member.Type} {member.Name} : {member.Semantics};");

            sb.DecrementIndent();
            sb.WriteLine("};");
            sb.WriteNewLine();
        }

        public void AddMember(string str)
        {
            _members.Add(new ShaderLabFieldMember(null, str, null, true));
        }

        public void AddMember(string type, string name, string? semantics)
        {
            _members.Add(new ShaderLabFieldMember(type, name, semantics, false));
        }

        private record ShaderLabFieldMember(string? Type, string Name, string? Semantics, bool IsMacro);
    }
}