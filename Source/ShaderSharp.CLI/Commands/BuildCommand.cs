using System;
using System.IO;
using System.Linq;
using System.Text.Json;

using Microsoft.Extensions.Logging;

using ShaderSharp.CLI.Models;
using ShaderSharp.Compiler;

namespace ShaderSharp.CLI.Commands
{
    public class BuildCommand
    {
        private readonly CompilerConfiguration _configuration;
        private readonly ILogger<CompilerInterface> _logger;
        private readonly string _out;
        private readonly string _project;
        private readonly string[] _references;
        private readonly string[] _sources;

        public BuildCommand(ILogger<CompilerInterface> logger, string project, string[] sources, string @out, string[] references)
        {
            _configuration = new CompilerConfiguration();
            _logger = logger;
            _project = project;
            _sources = sources;
            _out = @out;
            _references = references;
        }

        public int Run()
        {
            if (!ValidateOptions())
            {
                _logger.LogError("Invalid compiler options, please check compiler CLI arguments.");
                return 1;
            }

            var compiler = new ShaderSharpCompiler(_configuration.ToCompilerOptions());
            compiler.LockReferences();
            compiler.Compile();

            if (compiler.Errors.Count == 0)
                return 0;

            foreach (var error in compiler.Errors)
                _logger.LogError(error);

            return 1;
        }

        private bool ValidateOptions()
        {
            if (string.IsNullOrWhiteSpace(_project))
            {
                _configuration.Sources = _sources;
                _configuration.Out = _out;
                _configuration.References = _references;

                return ValidateRawArguments();
            }

            return ValidateProjectArgument();
        }

        private bool ValidateProjectArgument()
        {
            if (!File.Exists(_project))
                return false;

            try
            {
                var obj = JsonSerializer.Deserialize<CompilerConfiguration>(File.ReadAllText(_project));
                if (obj == null)
                    return false;

                _configuration.Sources = obj.Sources;
                _configuration.Out = obj.Out;
                _configuration.References = obj.References;

                return ValidateRawArguments();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to load pr parse project configuration.");
                return false;
            }
        }

        private bool ValidateRawArguments()
        {
            if (_configuration.Sources == null || _configuration.Sources.Length == 0)
                return false;

            if (string.IsNullOrWhiteSpace(_configuration.Out))
                return false;

            if (_configuration.References is { Length: > 0 })
                return _configuration.References.All(File.Exists);

            return true;
        }
    }
}