﻿using System;

namespace ShaderSharp.Compiler.Abstractions.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IncludeAttribute : Attribute
    {
        public string FilePath { get; }

        public IncludeAttribute(string filePath)
        {
            FilePath = filePath;
        }
    }
}