using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Statements
{
    internal class Statement : INestableStatement
    {
        private readonly bool _hasSemicolonToken;
        private readonly List<IStatement> _statements;

        public Statement(SyntaxToken semi)
        {
            _statements = new List<IStatement>();
            _hasSemicolonToken = semi.IsKind(SyntaxKind.SemicolonToken);
        }

        public Statement(SyntaxToken semi, params IStatement[] statements)
        {
            _statements = statements.ToList();
            _hasSemicolonToken = semi.IsKind(SyntaxKind.SemicolonToken);
        }

        public void WriteTo(SourceBuilder sb)
        {
            if (!sb.IsIndented)
                sb.WriteIndent();

            foreach (var statement in _statements)
                statement.WriteTo(sb);

            if (_hasSemicolonToken)
                sb.WriteSpan(";");
            sb.WriteNewLine();
        }

        public void AddSourcePart(INestableStatement statement)
        {
            _statements.Add(statement);
        }

        public void AddSourcePart(IStatement statement)
        {
            _statements.Add(statement);
        }


        public Expression IntoExpression()
        {
            var expression = new Expression();
            foreach (var statement in _statements)
                expression.AddSourcePart(statement);

            return expression;
        }
    }
}