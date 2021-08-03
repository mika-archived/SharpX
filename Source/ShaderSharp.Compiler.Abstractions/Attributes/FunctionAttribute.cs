﻿using System;

namespace ShaderSharp.Compiler.Abstractions.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class FunctionAttribute : Attribute
    {
        public string Name { get; }

        public FunctionAttribute(string name)
        {
            Name = name;
        }
    }
}