using System;
using System.IO;
using System.Linq;
using System.Text.Json;

using Microsoft.Extensions.Logging;

using SharpX.CLI.Models;
using SharpX.Compiler;

namespace SharpX.CLI.Commands
{
    public class BuildCommand
    {
        private readonly CompilerConfiguration _configuration;
        private readonly ILogger<CompilerInterface> _logger;
        private readonly string _out;
        private readonly string[] _plugins;
        private readonly string _project;
        private readonly string[] _references;
        private readonly string[] _sources;
        private readonly string _target;

        public BuildCommand(ILogger<CompilerInterface> logger, string project, string[] sources, string @out, string[] references, string[] plugins, string target)
        {
            _configuration = new CompilerConfiguration();
            _logger = logger;
            _project = project;
            _sources = sources;
            _out = @out;
            _references = references;
            _plugins = plugins;
            _target = target;
        }

        public int Run()
        {
            if (!ValidateOptions())
            {
                _logger.LogError("Invalid compiler options, please check compiler CLI arguments.");
                return 1;
            }

            var compiler = new SharpXCompiler(_configuration.ToCompilerOptions());
            compiler.LockReferences();
            compiler.LoadPluginModules();
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
                _configuration.Plugins = _plugins;

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
                _configuration.Target = obj.Target;
                _configuration.References = obj.References;
                _configuration.Plugins = obj.Plugins;

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

            if (string.IsNullOrWhiteSpace(_configuration.Target))
                return false;

            if (_configuration.References is { Length: > 0 } && _configuration.References.Any(w => !File.Exists(w)))
                return false;

            if (_configuration.Plugins is { Length: > 0 } && _configuration.Plugins.Any(w => !File.Exists(w)))
                return false;

            return true;
        }
    }
}