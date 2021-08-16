using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Text.Json;

using SharpX.Compiler.Composition.Attributes;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.Models.Plugin
{
    internal class SharpXPluginHost
    {
        private readonly ImmutableDictionary<string, JsonElement> _extraOptions;
        private readonly Dictionary<string, (ILanguageBackend, ILanguageBackendContext)> _implementations;
        private string? _identifier;

        public ILanguageBackend? CurrentLanguageBackend
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_identifier))
                    return null;
                if (_implementations.ContainsKey(_identifier))
                    return _implementations[_identifier].Item1;
                return null;
            }
        }

        public ILanguageBackendContext? CurrentLanguageBackendContext
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_identifier))
                    return null;
                if (_implementations.ContainsKey(_identifier))
                    return _implementations[_identifier].Item2;
                return null;
            }
        }

        public SharpXPluginHost(ImmutableDictionary<string, JsonElement> extraOptions)
        {
            _extraOptions = extraOptions;
            _implementations = new Dictionary<string, (ILanguageBackend, ILanguageBackendContext)>();
            _identifier = null;
        }

        public bool LoadPluginAtPath(string path)
        {
            try
            {
                var context = new SharpXPluginContext(path);
                var assembly = context.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
                foreach (var type in assembly.GetTypes())
                    switch (type)
                    {
                        case { } when type.GetCustomAttribute<LanguageBackendAttribute>() != null && typeof(ILanguageBackend).IsAssignableFrom(type):
                            LoadLanguageBackendImplementation(type);
                            break;
                    }

                return true;
            }
            catch (Exception e)
            {
                if (Debugger.IsAttached)
                    Debug.WriteLine(e.Message);

                return false;
            }
        }

        private void LoadLanguageBackendImplementation(Type t)
        {
            if (Activator.CreateInstance(t) is not ILanguageBackend backend)
                return; // ???

            var options = _extraOptions.ContainsKey(backend.Identifier) ? _extraOptions[backend.Identifier] : new JsonElement();
            _implementations.Add(backend.Identifier, (backend, new LanguageBackendContext(options)));
        }

        public bool HasLanguageBackend(string identifier)
        {
            return _implementations.ContainsKey(identifier);
        }

        public void InitializeLanguageBackend(string identifier)
        {
            if (!_implementations.ContainsKey(identifier))
                throw new InvalidOperationException();

            var (implementation, context) = _implementations[identifier];
            implementation.Initialize(context);

            SwitchLanguageBackend(identifier);
        }

        [MemberNotNull(nameof(_identifier))]
        public void SwitchLanguageBackend(string identifier)
        {
            if (!_implementations.ContainsKey(identifier))
                throw new InvalidOperationException();

            _identifier = identifier;
        }
    }
}