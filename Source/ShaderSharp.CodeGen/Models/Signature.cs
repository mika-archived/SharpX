using System.Collections.Generic;

namespace ShaderSharp.CodeGen.Models
{
    public record Signature
    {
        public List<Parameter> Parameters { get; } = new();

        public Parameter Returns { get; set; }
    }
}