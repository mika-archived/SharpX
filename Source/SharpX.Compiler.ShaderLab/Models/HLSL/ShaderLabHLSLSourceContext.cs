using System.Collections.Generic;

using SharpX.Compiler.Composition.Abstractions;

namespace SharpX.Compiler.ShaderLab.Models.HLSL
{
    // ReSharper disable once InconsistentNaming
    internal class ShaderLabHLSLSourceContext : VerifiableSourceContext
    {
        private readonly ShaderLabHLSLStructuredSourceBuilder _sb;

        public StructDeclaration? StructDeclaration { get; private set; }

        public FunctionDeclaration? FunctionDeclaration { get; private set; }

        public Dictionary<string, List<string>> FunctionDependencyTree { get; }

        public ShaderLabHLSLSourceContext()
        {
            _sb = new ShaderLabHLSLStructuredSourceBuilder();
            FunctionDependencyTree = new Dictionary<string, List<string>>();
        }

        public void AddGlobalMember(string str)
        {
            using (DisposableContextScope.Open<GlobalMemberDeclarationScope>(this, Scope))
                _sb.AddGlobalMember(new GlobalMember(null, str));
        }

        public void AddGlobalMember(string type, string name)
        {
            using (DisposableContextScope.Open<GlobalMemberDeclarationScope>(this, Scope))
                _sb.AddGlobalMember(new GlobalMember(type, name));
        }

        public override string ToSourceString()
        {
            return _sb.ToSource();
        }

        #region Function

        public void OpenFunction(string name, string returns)
        {
            EnterToNewScope(new HLSLFunctionDefinitionScope(Scope));

            FunctionDeclaration = new FunctionDeclaration(name, returns);
        }

        public void CloseFunction()
        {
            VerifyCurrentScope<HLSLFunctionDefinitionScope>();

            _sb.AddFunction(FunctionDeclaration!);
            FunctionDeclaration = null;

            GetOutFromCurrentScope();
        }

        #endregion

        #region Struct

        public void OpenStruct(string name)
        {
            EnterToNewScope(new HLSLStructDefinitionScope(Scope));

            StructDeclaration = new StructDeclaration(name);
        }

        public void CloseStruct()
        {
            VerifyCurrentScope<HLSLStructDefinitionScope>();

            _sb.AddStruct(StructDeclaration!);
            StructDeclaration = null;

            GetOutFromCurrentScope();
        }

        #endregion
    }
}