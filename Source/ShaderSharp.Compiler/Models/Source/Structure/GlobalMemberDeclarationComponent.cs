using ShaderSharp.Compiler.Models.Source.Structure.Interfaces;

namespace ShaderSharp.Compiler.Models.Source.Structure
{
    internal class GlobalMemberDeclarationComponent : IHeaderComponent
    {
        private readonly string _component;
        private readonly ComponentType _componentType;
        private readonly string _name;
        private readonly string _raw;

        public GlobalMemberDeclarationComponent(string component, string name)
        {
            _component = component;
            _name = name;
            _componentType = ComponentType.Structured;
        }

        public GlobalMemberDeclarationComponent(string raw)
        {
            _raw = raw;
            _componentType = ComponentType.Raw;
        }

        public void WriteTo(SourceWriter writer)
        {
            if (_componentType == ComponentType.Raw)
                writer.WriteLine(_raw);
            else
                writer.WriteLine($"{_component} {_name};");
        }

        public int Priority => 100;

        private enum ComponentType
        {
            Structured,

            Raw
        }
    }
}