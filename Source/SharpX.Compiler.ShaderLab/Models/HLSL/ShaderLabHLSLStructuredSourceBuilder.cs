using SharpX.Compiler.Composition.Abstractions;

namespace SharpX.Compiler.ShaderLab.Models.HLSL
{
    // ReSharper disable once InconsistentNaming
    internal class ShaderLabHLSLStructuredSourceBuilder : StructuredSourceBuilder
    {
        // Priority Ranges
        // 10000 - header comments
        // 1XXXX - header includes
        // 20000 - global member declarations
        // 3XXXX - global struct declarations
        // 4XXXX - global function declarations
        // 5XXXX - footer includes
        // 60000 - footer comments

        public void AddHeaderComments() { }

        public void AddHeaderInclude() { }

        public void AddGlobalMember() { }

        public void AddStruct(StructDeclaration declaration)
        {
            Components.Add(declaration);
        }

        public void AddFunction() { }

        public void AddFooterInclude() { }

        public void AddFooterComment() { }

        public override void CalcDependencyTree()
        {
            //
        }
    }
}