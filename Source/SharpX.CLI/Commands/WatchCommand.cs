using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;

using Microsoft.Extensions.Logging;

using SharpX.CLI.Models;
using SharpX.Compiler;

namespace SharpX.CLI.Commands
{
    public class WatchCommand
    {
        private readonly string? _baseDir;
        private readonly string[]? _excludes;
        private readonly string[]? _includes;
        private readonly object _lockObj = new();
        private readonly ILogger<CompilerInterface> _logger;
        private readonly string? _out;
        private readonly string[]? _plugins;
        private readonly string? _project;
        private readonly string[]? _references;
        private readonly string? _target;

        public WatchCommand(ILogger<CompilerInterface> logger, string? project, string? baseDir, string[]? includes, string[]? excludes, string? @out, string[]? references, string[]? plugins, string? target)
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
                    _logger.LogInformation("File change detected. Staring compilation...");

                    var compiler = new SharpXCompiler(configuration.ToCompilerOptions());
                    compiler.LockReferences();
                    compiler.LoadPluginModules();
                    compiler.CompileAsync().Wait(cancellationToken);

                    _logger.LogInformation($"Compilation finished with {compiler.Warnings.Count} warning(s) and {compiler.Errors.Count} error(s)");

                    if (compiler.Errors.Count == 0)
                        if (compiler.Warnings.Any())
                            foreach (var warning in compiler.Warnings)
                                _logger.LogWarning(warning);

                    foreach (var error in compiler.Errors)
                        _logger.LogError(error);
                }
                catch (Exception e)
                {
                    _logger.LogError(new EventId(1, "Incremental Compilation"), e, "Failed to compile");
                }
            }
        }
    }
}