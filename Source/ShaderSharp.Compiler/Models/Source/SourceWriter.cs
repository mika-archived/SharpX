using System.Text;

namespace ShaderSharp.Compiler.Models
{
    public class SourceWriter
    {
        private readonly StringBuilder _sb;
        private int _indent;

        public SourceWriter()
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

        public void WriteIndent(int number = 4)
        {
            _sb.Append("".PadLeft(_indent * number, ' '));
        }

        public void WriteLine(string str)
        {
            _sb.AppendLine(str);
        }

        public void WriteLineWithIndent(string str, int number = 4)
        {
            WriteIndent(number);
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

        public void WriteSpanWithIndent(string str, int number = 4)
        {
            WriteIndent(number);
            WriteSpan(str);
        }

        public void WriteSpace()
        {
            _sb.Append(" ");
        }

        public string ToSource()
        {
            return _sb.ToString();
        }
    }
}