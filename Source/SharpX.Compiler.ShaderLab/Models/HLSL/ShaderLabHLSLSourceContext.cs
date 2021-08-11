using SharpX.Compiler.Composition.Abstractions;

namespace SharpX.Compiler.ShaderLab.Models.HLSL
{
    // ReSharper disable once InconsistentNaming
    internal class ShaderLabHLSLSourceContext : VerifiableSourceContext
    {
        private readonly ShaderLabHLSLStructuredSourceBuilder _sb;

        public StructDeclaration? StructDeclaration { get; private set; }

        public ShaderLabHLSLSourceContext()
        {
            _sb = new ShaderLabHLSLStructuredSourceBuilder();
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