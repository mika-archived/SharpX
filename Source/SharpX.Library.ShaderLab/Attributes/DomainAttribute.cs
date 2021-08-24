using System;

using SharpX.Library.ShaderLab.Attributes.Internal;
using SharpX.Library.ShaderLab.Enums;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DomainAttribute : SourcePartAttribute
    {
        public Domain Domain { get; }

        public DomainAttribute(Domain domain)
        {
            Domain = domain;
        }

        public override string ToSourcePart()
        {
            var domain = Domain switch
            {
                Domain.Three => "3",
                Domain.Four => "4",
                Domain.Isoline => "isoline",
                _ => throw new ArgumentOutOfRangeException()
            }
                ;
            return $"domain({domain})";
        }
    }
}