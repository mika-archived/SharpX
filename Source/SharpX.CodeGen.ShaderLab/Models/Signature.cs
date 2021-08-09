using System.Collections.Generic;

namespace SharpX.CodeGen.ShaderLab.Models
{
    public record Signature(List<Parameter> Parameters, Parameter Returns) { }
}