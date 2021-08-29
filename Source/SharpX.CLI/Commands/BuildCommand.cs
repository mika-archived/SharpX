using System.Linq;

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
            if (!ParameterValidator.ValidateOptions(_logger, _project, _baseDir, _includes, _excludes, _out, _references, _plugins, _target))
            {
                _logger.LogError("Invalid compiler options, please check compiler CLI arguments.");
                return 1;
            }

            var configuration = ParameterValidator.CreateConfiguration(_project, _baseDir, _includes, _excludes, _out, _references, _plugins, _target, null);
            var compiler = new SharpXCompiler(configuration.ToCompilerOptions());
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
    }
}