using System;
using System.Collections.Generic;

using ShaderSharp.Compiler.Models.Source;

namespace ShaderSharp.Compiler.Models
{
    public class ProjectContext
    {
        private readonly Dictionary<string, SourceContext> _contexts;

        public SourceContext Default => _contexts["source"];

        public ProjectContext()
        {
            _contexts = new Dictionary<string, SourceContext> { { "source", new SourceContext() } };
        }

        public SourceContext AddContext(string name)
        {
            if (_contexts.ContainsKey(name))
                throw new ArgumentException($"{nameof(name)} is already exists in context");

            _contexts.Add(name, new SourceContext());

            return GetContext(name);
        }

        public bool HasContext(string name)
        {
            return _contexts.ContainsKey(name);
        }

        public SourceContext GetContext(string name)
        {
            if (_contexts.ContainsKey(name))
                return _contexts[name];

            throw new ArgumentException($"{nameof(name)} is not found in context");
        }

        public string Flush(string name)
        {
            var context = GetContext(name);
            return context.ToSource();
        }

        public Dictionary<string, string> FlushAll()
        {
            var dict = new Dictionary<string, string>();
            foreach (var name in _contexts.Keys)
            {
                var source = Flush(name);
                if (string.IsNullOrWhiteSpace(source))
                    continue;

                dict.Add(name, source);
            }

            return dict;
        }
    }
}