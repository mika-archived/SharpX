using System.Linq;
using System.Security.Cryptography;
using System.Text;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Statements.Structured
{
    internal class Include : IStructuredComponent
    {
        private static readonly MD5 HashProvider = MD5.Create();
        private readonly string _include;

        public Include(string include)
        {
            _include = include;

            Name = $"Include_{include}";
        }

        public int Priority { get; set; } = 1000;

        public string Name { get; }

        public void WriteTo(SourceBuilder sb)
        {
            var guardHash = string.Concat(HashProvider.ComputeHash(Encoding.UTF8.GetBytes(_include)).Select(w => $"{w:X2}"));

            sb.WriteLine($@"#ifndef  INCLUDE_GUARD_{guardHash}");
            sb.WriteLine($@"#define  INCLUDE_GUARD_{guardHash}");
            sb.WriteLine($@"#include ""{_include}""");
            sb.WriteLine($@"#endif   // Auto-Generated Include Guard for {_include}");
            sb.WriteNewLine();
        }
    }
}