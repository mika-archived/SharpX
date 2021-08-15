﻿using SharpX.CodeGen.ShaderLab.Attributes;
using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Library.ShaderLab.Functions
{
    [AutoGenerated("Functions/Resources/builtin.txt")]
    public static partial class Builtin
    {
        [Function("mul")]
        public static extern T Mul<T>(object a, object b);

		[Function("transpose")]
		public static extern T Transpose<T>(object a);
	}
}