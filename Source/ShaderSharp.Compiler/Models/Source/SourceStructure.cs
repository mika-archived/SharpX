using System.Collections.Generic;

using ShaderSharp.Compiler.Models.Source.Structure.Interfaces;

namespace ShaderSharp.Compiler.Models.Source
{
    public class SourceStructure
    {
        private readonly List<IFooterComponent> _footers;
        private readonly List<IHeaderComponent> _headers;
        private readonly List<IStructDeclarationComponent> _structDeclarations;
        private readonly SourceWriter _writer;

        public SourceStructure()
        {
            _headers = new List<IHeaderComponent>();
            _footers = new List<IFooterComponent>();
            _structDeclarations = new List<IStructDeclarationComponent>();
            _writer = new SourceWriter();
        }

        public void AddHeader(IHeaderComponent header)
        {
            _headers.Add(header);
        }

        public void AddStructDeclaration(IStructDeclarationComponent declaration)
        {
            _structDeclarations.Add(declaration);
        }

        public void AddFunctionDeclaration() { }

        public void AddFooter(IFooterComponent footer)
        {
            _footers.Add(footer);
        }

        public void CalcDependencyTree() { }

        public string ToSource()
        {
            foreach (var header in _headers)
                header.WriteTo(_writer);

            foreach (var declaration in _structDeclarations)
                declaration.WriteTo(_writer);

            foreach (var footer in _footers)
                footer.WriteTo(_writer);

            return _writer.ToSource();
        }
    }
}