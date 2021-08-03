using System.Collections.Generic;
using System.Linq;

using ShaderSharp.Compiler.Models.Source.Structure.Interfaces;

namespace ShaderSharp.Compiler.Models.Source.Structure
{
    public class StructDeclarationComponent : IStructDeclarationComponent
    {
        private readonly HashSet<ShaderLabFieldMember> _members;
        private readonly string _name;

        public StructDeclarationComponent(string name)
        {
            _name = name;
            _members = new HashSet<ShaderLabFieldMember>();
        }

        public void WriteTo(SourceWriter writer)
        {
            writer.WriteLine($"struct {_name} {{");
            writer.IncrementIndent();

            foreach (var member in _members)
                if (member.IsMacro)
                    writer.WriteLineWithIndent(member.Name);
                else
                    writer.WriteLineWithIndent(member.HasSemantics ? $"{member.Type} {member.Name} : {member.Semantics};" : $"{member.Type} {member.Name};");

            writer.DecrementIndent();
            writer.WriteLine("};");
            writer.WriteNewLine();
        }

        public void AddMemberDeclaration(string str)
        {
            _members.Add(new ShaderLabFieldMember { Name = str, IsMacro = true });
        }

        public void AddMemberDeclaration(string t, string name, params KeyValuePair<string, string>[] extras)
        {
            var member = new ShaderLabFieldMember { Type = t, Name = name, IsMacro = false };
            if (extras.Any(w => w.Key == "Semantics"))
                _members.Add(member with { Semantics = extras.First(w => w.Key == "Semantics").Value });
            else
                _members.Add(member);
        }

        private record ShaderLabFieldMember
        {
            public string Type { get; init; }

            public string Name { get; init; }

            public string Semantics { get; init; }

            public bool HasSemantics => !string.IsNullOrWhiteSpace(Semantics);

            public bool IsMacro { get; init; }
        }
    }
}