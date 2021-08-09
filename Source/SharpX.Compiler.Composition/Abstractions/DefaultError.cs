using Microsoft.CodeAnalysis.CSharp;

using SharpX.Compiler.Composition.Interfaces;
using SharpX.Compiler.Extensions;

namespace SharpX.Compiler.Composition.Abstractions
{
    public class DefaultError : IError
    {
        private readonly string _msg;
        private readonly CSharpSyntaxNode _node;

        public DefaultError(CSharpSyntaxNode node, string message)
        {
            _node = node;
            _msg = message;
        }

        public string GetMessage()
        {
            return $"{_msg} at {_node.ToLocationString()}";
        }
    }
}