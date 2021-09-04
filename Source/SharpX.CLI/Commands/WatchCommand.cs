using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using NLog;

using SharpX.CLI.Models;
using SharpX.Compiler;

namespace SharpX.CLI.Commands
{
    internal class WatchCommand
    {
        private readonly object _lockObj = new();
        private readonly Logger _logger;
        private readonly string _project;
        private SharpXCompiler? _compiler;

        public WatchCommand(Logger logger, WatchCommandArguments args)
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
            var watcher = new FileSystemWatcher(configuration.BaseDir)
            {
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = "*",
                IncludeSubdirectories = true,
                EnableRaisingEvents = true
            };

            _compiler = new SharpXCompiler(configuration.ToCompilerOptions());
            _compiler.LockReferences();
            _compiler.LoadPluginModules();
            _compiler.CompileAsync(configuration.ToItems().ToImmutableArray()).Wait();

            var source = new CancellationTokenSource();
            var handler = CreateThrottleEventHandler(TimeSpan.FromSeconds(1), (_, _) =>
            {
                _logger.Info("File change detected. Staring compilation...");
                CompileAsync(configuration, source.Token);
            });

            watcher.Created += handler;
            watcher.Changed += handler;
            watcher.Deleted += handler;
            Console.CancelKeyPress += (_, _) => source.Cancel();

            CompileAsync(configuration, source.Token);

            while (!source.IsCancellationRequested)
                Console.ReadKey(true);

            watcher.Created -= handler;
            watcher.Changed -= handler;
            watcher.Deleted -= handler;
            watcher.Dispose();

            return 0;
        }

        private FileSystemEventHandler CreateThrottleEventHandler(TimeSpan throttle, FileSystemEventHandler handler)
        {
            var isThrottling = false;
            return (sender, e) =>
            {
                if (isThrottling)
                    return;

                handler.Invoke(sender, e);
                isThrottling = true;

                Task.Delay(throttle).ContinueWith(_ => isThrottling = false);
            };
        }

        private void CompileAsync(CompilerConfiguration configuration, CancellationToken cancellationToken)
        {
            if (_compiler == null)
                return;

            lock (_lockObj)
            {
                try
                {
                    _compiler.CompileAsync(configuration.ToItems().ToImmutableArray()).Wait(cancellationToken);

                    _logger.Info($"Compilation finished with {_compiler.Warnings.Count} warning(s) and {_compiler.Errors.Count} error(s)");

                    foreach (var warning in _compiler.Warnings)
                        _logger.Warn(warning);

                    foreach (var error in _compiler.Errors)
                        _logger.Error(error);
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Failed to compile");
                }
            }
        }
    }
}