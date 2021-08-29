using System;

using SharpX.Library.ShaderLab.Attributes.Internal;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PatchConstantFuncAttribute : SourcePartAttribute
    {
        public string Function { get; }

        public PatchConstantFuncAttribute(string function)
        {
            Function = function;
        }

        public override string ToSourcePart()
        {
            return $"patchconstantfunc(\"{Function}\")";
        }
    }
}