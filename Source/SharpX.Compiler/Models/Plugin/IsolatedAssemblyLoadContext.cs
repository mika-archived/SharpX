﻿using System;
using System.Reflection;
using System.Runtime.Loader;

namespace SharpX.Compiler.Models.Plugin
{
    public class IsolatedAssemblyLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public IsolatedAssemblyLoadContext(string path)
        {
            _resolver = new AssemblyDependencyResolver(path);
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            var path = _resolver.ResolveAssemblyToPath(assemblyName);
            if (string.IsNullOrWhiteSpace(path))
                return null;

            return LoadFromAssemblyPath(path);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var path = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (string.IsNullOrWhiteSpace(path))
                return IntPtr.Zero;

            return LoadUnmanagedDllFromPath(path);
        }
    }
}