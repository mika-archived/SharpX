using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        private readonly string? _baseDir;
        private readonly string[]? _excludes;
        private readonly string[]? _includes;
        private readonly ILogger<CompilerInterface> _logger;
        private readonly string? _out;
        private readonly string[]? _plugins;
        private readonly string? _project;
        private readonly string[]? _references;
        private readonly string? _target;
        private CompilerConfiguration? _configuration;

        public BuildCommand(ILogger<CompilerInterface> logger, string? project, string? baseDir, string[]? includes, string[]? excludes, string? @out, string[]? references, string[]? plugins, string? target)
        {
            _logger = logger;
            _project = project;
            _baseDir = baseDir;
            _includes = includes;
            _excludes = excludes;
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
            compiler.CompileAsync().Wait();

            if (compiler.Errors.Count == 0)
            {
                if (compiler.Warnings.Any())
                    foreach (var warning in compiler.Warnings)
                        _logger.LogWarning(warning);

                return 0;
            }

            foreach (var error in compiler.Errors)
                _logger.LogError(error);

            return 1;
        }

        [MemberNotNullWhen(true, nameof(_configuration))]
        private bool ValidateOptions()
        {
            if (string.IsNullOrWhiteSpace(_project))
                return ValidateRawArguments(_baseDir, _includes, _excludes, _out, _references, _plugins, _target, new Dictionary<string, JsonElement>());

            return ValidateProjectArgument();
        }

        [MemberNotNullWhen(true, nameof(_configuration))]
        private bool ValidateProjectArgument()
        {
            if (!File.Exists(_project))
                return false;

            try
            {
                var obj = JsonSerializer.Deserialize<CompilerConfiguration>(File.ReadAllText(_project));
                if (obj == null)
                    return false;

                return ValidateRawArguments(obj.BaseDir, obj.Includes, obj.Excludes, obj.Out, obj.References, obj.Plugins, obj.Target, obj.CustomOptions);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to load or parse project configuration.");
                return false;
            }
        }

        [MemberNotNullWhen(true, nameof(_configuration))]
        private bool ValidateRawArguments(string? baseDir, string[]? includes, string[]? excludes, string? @out, string[]? references, string[]? plugins, string? target, Dictionary<string, JsonElement>? customOptions)
        {
            if (string.IsNullOrWhiteSpace(baseDir) || !Directory.Exists(baseDir))
                return false;

            if (includes == null || includes.Length == 0)
                return false;

            if (string.IsNullOrWhiteSpace(@out))
                return false;

            if (references is { Length: > 0 } && references.Any(w => !File.Exists(w)))
                return false;

            if (plugins is { Length: > 0 } && plugins.Any(w => !File.Exists(w)))
                return false;

            if (string.IsNullOrWhiteSpace(target))
                return false;

            _configuration = new CompilerConfiguration(Path.GetFullPath(baseDir), includes, excludes ?? Array.Empty<string>(), references ?? Array.Empty<string>(), plugins ?? Array.Empty<string>(), @out, target);
            _configuration = _configuration with { CustomOptions = customOptions ?? new Dictionary<string, JsonElement>() };
            return true;
        }
    }
}