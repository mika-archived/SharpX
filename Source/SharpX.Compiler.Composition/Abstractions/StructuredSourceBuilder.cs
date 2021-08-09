using System.Collections.Generic;
using System.Linq;

using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.Composition.Abstractions
{
    public abstract class StructuredSourceBuilder
    {
        private readonly SourceBuilder _sb;

        protected List<IStructuredComponent> Components { get; }

        protected StructuredSourceBuilder()
        {
            _sb = new SourceBuilder();
            Components = new List<IStructuredComponent>();
        }

        public abstract void CalcDependencyTree();

        public string ToSource()
        {
            CalcDependencyTree();

            foreach (var component in Components.OrderBy(w => w.Priority))
                component.WriteTo(_sb);

            return _sb.ToSource();
        }
    }
}