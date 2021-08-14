using System.Collections.Generic;
using System.Linq;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Statements
{
    internal class Constructor : INestableStatement
    {
        private readonly List<IStatement> _arguments;
        private readonly string _ctor;

        public Constructor(string ctor)
        {
            _ctor = ctor;
            _arguments = new List<IStatement>();
        }

        public void WriteTo(SourceBuilder sb)
        {
            sb.WriteSpan($"{_ctor}(");

            foreach (var (argument, i) in _arguments.Select((w, i) => (w, i)))
            {
                if (i > 0)
                    sb.WriteSpan(", ");
                argument.WriteTo(sb);
            }

            sb.WriteSpan(")");
        }

        public void AddSourcePart(INestableStatement statement)
        {
            _arguments.Add(statement);
        }

        public void AddSourcePart(IStatement statement)
        {
            _arguments.Add(statement);
        }
    }
}