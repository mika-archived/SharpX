using System;

namespace ShaderSharp.Compiler.Abstractions.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class ExternalAttribute : Attribute { }
}