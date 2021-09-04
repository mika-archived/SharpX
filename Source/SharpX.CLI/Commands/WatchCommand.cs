using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading;

using NLog;

using SharpX.CLI.Models;
using SharpX.Compiler;

namespace SharpX.CLI.Commands
{
    public class WatchCommand
    {
        private readonly object _lockObj = new();
        private readonly Logger _logger;
        private readonly string _project;

        public WatchCommand(Logger logger, string project)
        {
            _logger = logger;
            _project = project;
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

            var source = new CancellationTokenSource();

            // ReSharper disable AccessToDisposedClosure
            var observable = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h =>
            {
                watcher.Created += h;
                watcher.Changed += h;
                watcher.Deleted += h;
            }, h =>
            {
                watcher.Created -= h;
                watcher.Changed -= h;
                watcher.Deleted -= h;
            }).Throttle(TimeSpan.FromSeconds(1)).Subscribe(_ => CompileAsync(configuration, source.Token));

            Console.CancelKeyPress += (_, _) => source.Cancel();

            while (!source.IsCancellationRequested)
                Console.ReadKey(true);

            observable.Dispose();
            watcher.Dispose();

            return 0;
        }

        private void CompileAsync(CompilerConfiguration configuration, CancellationToken cancellationToken)
        {
            lock (_lockObj)
            {
                try
                {
                    _logger.Info("File change detected. Staring compilation...");

                    var compiler = new SharpXCompiler(configuration.ToCompilerOptions());
                    compiler.LockReferences();
                    compiler.LoadPluginModules();
                    compiler.CompileAsync().Wait(cancellationToken);

                    _logger.Info($"Compilation finished with {compiler.Warnings.Count} warning(s) and {compiler.Errors.Count} error(s)");

                    foreach (var warning in compiler.Warnings)
                        _logger.Warn(warning);

                    foreach (var error in compiler.Errors)
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