using ShaderSharp.Compiler.Models.Source.Structure.Interfaces;

namespace ShaderSharp.Compiler.Models.Source.Structure
{
    public class IncludeComponent : IHeaderComponent, IFooterComponent
    {
        private readonly string _file;

        public IncludeComponent(string file)
        {
            _file = file;
        }

        public int Priority => -10000;

        public void WriteTo(SourceWriter writer)
        {
            writer.WriteLine($"#include <{_file}>");
        }
    }
}