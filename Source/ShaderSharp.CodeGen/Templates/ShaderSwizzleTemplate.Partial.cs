using System.Linq;

namespace ShaderSharp.CodeGen.Templates
{
    public partial class ShaderSwizzleTemplate
    {
        internal string ClassName { get; set; }

        internal string[] Components { get; set; }

        internal int MaxLength { get; set; }

        private string GetReturnType(int i)
        {
            return $"T{i}";
        }

        private string GetAccessors(string signature)
        {
            if (signature.Distinct().Count() != signature.Length)
                return "{ get; }";
            return "{ get; set; }";
        }
    }
}