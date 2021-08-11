using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL
{
    internal class GlobalMember : IStructuredComponent
    {
        private readonly string _name;
        private readonly string? _type;

        public GlobalMember(string? type, string name)
        {
            _type = type;
            _name = name;

            Name = $"Global_{_name}";
        }

        public int Priority { get; set; } = 2000;

        public string Name { get; }

        public void WriteTo(SourceBuilder sb)
        {
            if (string.IsNullOrWhiteSpace(_type))
                sb.WriteLine(_name);
            else
                sb.WriteLine($"{_type} {_name};");
        }
    }
}