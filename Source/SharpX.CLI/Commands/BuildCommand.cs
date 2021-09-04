using System.Collections.Immutable;
using System.Linq;

using NLog;

using SharpX.CLI.Models;
using SharpX.Compiler;

namespace SharpX.CLI.Commands
{
    internal class BuildCommand
    {
        private readonly Logger _logger;
        private readonly string _project;

        public BuildCommand(Logger logger, BuildCommandArguments args)
        {
            _logger = logger;
            _project = args.Project;
        }

        public int Run()
        {
            if (!ParameterValidator.ValidateOptions(_logger, _project))
            {
                _logger.Error("Invalid compiler options, please check compiler CLI arguments.");
                return 1;
            }

            var configuration = ParameterValidator.CreateConfiguration(_project);
            var compiler = new SharpXCompiler(configuration.ToCompilerOptions());
            compiler.LockReferences();
            compiler.LoadPluginModules();
            compiler.CompileAsync(configuration.ToItems().ToImmutableArray()).Wait();

            if (compiler.Errors.Count == 0)
            {
                if (compiler.Warnings.Any())
                    foreach (var warning in compiler.Warnings)
                        _logger.Warn(warning);

                return 0;
            }

            foreach (var error in compiler.Errors)
                _logger.Error(error);

            return 1;
        }
    }
}