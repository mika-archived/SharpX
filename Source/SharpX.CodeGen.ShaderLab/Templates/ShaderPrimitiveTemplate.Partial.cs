using System.Linq;
using System.Text.RegularExpressions;

namespace SharpX.CodeGen.ShaderLab.Templates
{
    public partial class ShaderPrimitiveTemplate
    {
        private static readonly Regex MatrixRegex = new("^(?<a>\\d)x(?<b>\\d)$", RegexOptions.Compiled);
        private static readonly Regex VectorRegex = new("^\\d$", RegexOptions.Compiled);
        internal string ComponentName { get; }

        internal string ClassName { get; }

        internal string ClassNameWithoutComponent
        {
            get
            {
                if (IsVector())
                    return ClassName.Substring(0, ClassName.Length - 1);
                if (IsMatrix())
                    return ClassName.Substring(0, ClassName.Length - 3);
                return ClassName;
            }
        }

        internal string BaseClassName
        {
            get
            {
                if (IsMatrix())
                    return ClassName.Substring(0, ClassName.Length - 2);
                return ClassName;
            }
        }

        internal int BaseClassDuplication
        {
            get
            {
                if (IsMatrix())
                    return int.Parse(ClassName.Last().ToString());
                return 1;
            }
        }

        internal string CSharpPrimitive { get; }

        internal string? Inheritance { get; set; }

        internal string Template { get; }

        public ShaderPrimitiveTemplate(string name, string cls, string template, string primitive)
        {
            ComponentName = name;
            ClassName = cls;
            Template = template;
            CSharpPrimitive = primitive;
        }

        private bool IsVector()
        {
            return VectorRegex.IsMatch(Template);
        }

        internal bool IsMatrix()
        {
            return MatrixRegex.IsMatch(Template);
        }

        internal int MatrixConstructorCount()
        {
            if (IsMatrix())
            {
                var m = MatrixRegex.Match(Template);
                return int.Parse(m.Groups["a"].Value) * int.Parse(m.Groups["b"].Value);
            }

            return 0;
        }
    }
}