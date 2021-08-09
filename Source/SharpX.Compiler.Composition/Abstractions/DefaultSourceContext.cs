using System.Text;

using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.Composition.Abstractions
{
    public class DefaultSourceContext : ISourceContext
    {
        private readonly StringBuilder _sb;

        public DefaultSourceContext()
        {
            _sb = new StringBuilder();
        }

        public string ToSourceString()
        {
            return _sb.ToString();
        }
    }
}