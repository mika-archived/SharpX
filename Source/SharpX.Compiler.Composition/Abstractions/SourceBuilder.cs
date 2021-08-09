using System.Text;

namespace SharpX.Compiler.Composition.Abstractions
{
    public class SourceBuilder
    {
        private readonly StringBuilder _sb;

        private int _indent;

        public SourceBuilder()
        {
            _sb = new StringBuilder();
            _indent = 0;
        }

        public void IncrementIndent()
        {
            _indent++;
        }

        public void DecrementIndent()
        {
            _indent--;
        }

        public void WriteIndent(int chars = 4)
        {
            _sb.Append("".PadLeft(_indent * chars, ' '));
        }

        public void WriteLine(string str)
        {
            _sb.AppendLine(str);
        }

        public void WriteLineWithIndent(string str, int chars = 4)
        {
            WriteIndent(chars);
            WriteLine(str);
        }

        public void WriteNewLine()
        {
            _sb.AppendLine();
        }

        public void WriteSpan(string str)
        {
            _sb.Append(str);
        }

        public void WriteSpanWithIndent(string str, int chars = 4)
        {
            WriteIndent(chars);
            WriteSpan(str);
        }

        public void WriteSpace()
        {
            _sb.Append(" ");
        }

        public string ToSource()
        {
            return _sb.ToString().Trim();
        }
    }
}