using System;

namespace ShaderSharp.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ExportAttribute : Attribute
    {
        public string FilePath { get; }

        public ExportAttribute(string filePath)
        {
            FilePath = filePath;
        }
    }
}