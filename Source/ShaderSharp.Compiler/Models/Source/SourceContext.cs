using System.Collections.Generic;

using ShaderSharp.Compiler.Models.Context;
using ShaderSharp.Compiler.Models.Source.Structure;
using ShaderSharp.Compiler.Models.Source.Structure.Interfaces;

namespace ShaderSharp.Compiler.Models.Source
{
    public class SourceContext
    {
        private readonly SourceStructure _structure;
        private IStructDeclarationComponent _structDeclaration;

        public ContextScope Scope { get; set; }

        public SourceContext()
        {
            _structure = new SourceStructure();
            Scope = new RootContextScope();
        }

        public void Include(string file, bool toHeader = true)
        {
            using (IncludeScope.Open(this, Scope))
            {
                if (toHeader)
                    _structure.AddHeader(new IncludeComponent(file));
                else
                    _structure.AddFooter(new IncludeComponent(file));
            }
        }

        public string ToSource()
        {
            _structure.CalcDependencyTree();
            return _structure.ToSource();
        }

        private void EnterNewScope(ContextScope scope)
        {
            Scope = scope;
            Scope.VerifyIntegrity();
        }

        private void VerifyCurrentScope<T>()
        {
            Scope.VerifyIntegrity<T>();
        }

        private void GetOutScope()
        {
            Scope = Scope.Close();
        }

        #region Struct

        public void OpenStruct(string name)
        {
            EnterNewScope(new StructDefinitionScope(Scope));

            _structDeclaration = new StructDeclarationComponent(name);
        }

        public void AddStructMember(string component, string name, params KeyValuePair<string, string>[] extras)
        {
            VerifyCurrentScope<StructDefinitionScope>();

            _structDeclaration.AddMemberDeclaration(component, name, extras);
        }

        public void CloseStruct()
        {
            VerifyCurrentScope<StructDefinitionScope>();

            _structure.AddStructDeclaration(_structDeclaration);

            GetOutScope();
        }

        #endregion
    }
}