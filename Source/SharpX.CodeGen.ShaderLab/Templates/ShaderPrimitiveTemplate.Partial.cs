﻿using System.Text.RegularExpressions;

namespace SharpX.CodeGen.ShaderLab.Templates
{
    public partial class ShaderPrimitiveTemplate
    {
        private static readonly Regex MatrixRegex = new("^\\dx\\d$", RegexOptions.Compiled);

        internal string ComponentName { get; }

        internal string ClassName { get; }

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
            return int.TryParse(Template, out _);
        }

        internal bool IsMatrix()
        {
            return MatrixRegex.IsMatch(Template);
        }
    }
}