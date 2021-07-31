using System.Text.RegularExpressions;

namespace ShaderSharp.CodeGen.Templates
{
    public partial class ShaderPrimitiveTemplate
    {
        private static readonly Regex MatrixRegex = new("^\\dx\\d$", RegexOptions.Compiled);

        internal string ComponentName { get; set; }

        internal string ClassName { get; set; }

        internal string CSharpPrimitive { get; set; }

        internal string Inheritance { get; set; }

        internal string Template { get; set; }

        private bool IsVector()
        {
            return int.TryParse(Template, out _);
        }

        internal bool IsMatrix()
        {
            return MatrixRegex.IsMatch(Template);
        }
    }
}