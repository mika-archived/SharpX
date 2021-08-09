using System;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, Inherited = false)]
    public class IncludeAttribute : Attribute
    {
        public string FileName { get; }

        public IncludeAttribute(string filename)
        {
            FileName = filename;
        }
    }
}