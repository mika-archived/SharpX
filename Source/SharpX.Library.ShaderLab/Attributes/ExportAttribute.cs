using System;
using System.IO;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, Inherited = false)]
    public class ExportAttribute : Attribute
    {
        public string Source { get; }

        public ExportAttribute(string source)
        {
            Source = source;
        }

        public bool IsValidSourceName()
        {
            if (string.IsNullOrWhiteSpace(Source))
                return false;
            if (Source.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                return false;
            return true;
        }
    }
}