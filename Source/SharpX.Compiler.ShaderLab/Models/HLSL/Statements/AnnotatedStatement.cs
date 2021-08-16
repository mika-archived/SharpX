using System;
using System.Collections.Generic;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Statements
{
    public class AnnotatedStatement : INestableStatement
    {
        private readonly List<string> _annotations;
        private readonly List<IStatement> _statements;

        public AnnotatedStatement()
        {
            _annotations = new List<string>();
            _statements = new List<IStatement>();
        }

        public void WriteTo(SourceBuilder sb)
        {
            foreach (var annotation in _annotations)
                sb.WriteLineWithIndent($"[{annotation}]");

            foreach (var statement in _statements)
                statement.WriteTo(sb);
        }

        public void AddSourcePart(INestableStatement statement)
        {
            _statements.Add(statement);
        }

        public void AddSourcePart(IStatement statement)
        {
            _statements.Add(statement);
        }

        public void AddAnnotation(string annotation)
        {
            _annotations.Add(annotation);
        }
    }
}