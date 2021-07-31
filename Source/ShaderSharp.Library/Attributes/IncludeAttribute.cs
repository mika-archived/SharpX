using System;

namespace ShaderSharp.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IncludeAttribute : Attribute
    {
        public string FilePath { get; }

        public IncludeAttribute(string filePath)
        {
            FilePath = filePath;
        }
    }
}