using SharpX.Compiler.Composition.Abstractions;

namespace SharpX.Compiler.ShaderLab.Models.HLSL
{
    // ReSharper disable once InconsistentNaming
    internal class ShaderLabHLSLSourceContext : VerifiableSourceContext
    {
        private readonly ShaderLabHLSLStructuredSourceBuilder _sb;

        // temporary begin

        private StructDeclaration? _struct;

        // temporary end

        public ShaderLabHLSLSourceContext()
        {
            _sb = new ShaderLabHLSLStructuredSourceBuilder();
        }

        public void AddGlobalMember(string str) { }

        public void AddGlobalMember(string type, string name) { }

        public override string ToSourceString()
        {
            return _sb.ToSource();
        }

        #region Struct

        public void OpenStruct(string name)
        {
            EnterToNewScope(new HLSLStructDefinitionScope(Scope));

            _struct = new StructDeclaration(name);
        }

        public void AddMemberToStruct(string str)
        {
            VerifyCurrentScope<HLSLStructDefinitionScope>();

            _struct!.AddMember(str);
        }

        public void AddMemberToStruct(string type, string name, string? semantics)
        {
            VerifyCurrentScope<HLSLStructDefinitionScope>();

            _struct!.AddMember(type, name, semantics);
        }

        public void CloseStruct()
        {
            _sb.AddStruct(_struct!);
            _struct = null;

            GetOutFromCurrentScope();
        }

        #endregion
    }
}