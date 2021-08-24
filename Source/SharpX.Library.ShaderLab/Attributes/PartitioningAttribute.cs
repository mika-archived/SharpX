using System;

using SharpX.Library.ShaderLab.Attributes.Internal;
using SharpX.Library.ShaderLab.Enums;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PartitioningAttribute : SourcePartAttribute
    {
        public Partitioning Partitioning { get; }

        public PartitioningAttribute(Partitioning partitioning)
        {
            Partitioning = partitioning;
        }

        public override string ToSourcePart()
        {
            var partitioning = Partitioning switch {
                Partitioning.Integer => "integer",
                Partitioning.FractionalEven => "fractional_even",
                Partitioning.FractionalOdd => "fractional_odd",
                Partitioning.Pow2 => "pow2",
                _ => throw new ArgumentOutOfRangeException()
            };

            return $"partitioning({partitioning})";
        }
    }
}