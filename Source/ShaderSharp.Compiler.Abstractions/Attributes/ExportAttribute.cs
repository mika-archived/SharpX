using System;

namespace ShaderSharp.Compiler.Abstractions.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
    public class ExportAttribute : Attribute
    {
        public string FilePath { get; }

        public ExportAttribute(string filePath)
        {
            FilePath = filePath;
        }
    }
}