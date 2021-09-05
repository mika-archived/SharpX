using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

using NLog;

using SharpX.Compiler;
using SharpX.Compiler.Models;

namespace SharpX.CLI.Models
{
    internal record Project(string Version, string? BaseDir, string[]? Includes, string[]? Excludes, string[]? References, string[]? Plugins, string? Out, string? Target) : IDisposable
    {
        private Action? _disposer;

        // ReSharper disable once CollectionNeverUpdated.Global
        [JsonExtensionData]
        public Dictionary<string, JsonElement> CustomOptions { get; init; } = new();

        public void Dispose()
        {
            _disposer?.Invoke();
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(BaseDir) || !Directory.Exists(BaseDir))
                throw new ArgumentException("invalid base directory: directory is empty or not exists in file system");

            if (Includes?.Length == 0)
                throw new ArgumentException("invalid source items: includes must be one or more items.");

            if (string.IsNullOrWhiteSpace(Out))
                throw new ArgumentException("invalid out directory: directory is empty");

            if (References is { Length: > 0 } && References.Any(w => !File.Exists(w)))
                throw new ArgumentException("invalid references: one or more references is not exists on file system");

            if (Plugins is { Length: > 0 } && Plugins.Any(w => !File.Exists(w)))
                throw new ArgumentException("invalid plugin references: one or more plugin references is not exists on file system");

            if (string.IsNullOrWhiteSpace(Target))
                throw new ArgumentException("invalid target: target is empty");
        }

        public SharpXCompilerOptions ToCompilerOptions()
        {
            return new SharpXCompilerOptions(BaseDir!, References!.ToImmutableArray(), Plugins!.ToImmutableArray(), Out!, Target!, CustomOptions.ToImmutableDictionary());
        }

        public IEnumerable<string> ToSourceItems()
        {
            var matcher = new Matcher();
            matcher.AddIncludePatterns(Includes);
            matcher.AddExcludePatterns(Excludes ?? Array.Empty<string>());

            var items = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(BaseDir!)));
            return items.Files.Select(w => w.Path);
        }

        public bool Build(Logger logger)
        {
            var compiler = new SharpXCompiler(ToCompilerOptions());
            compiler.LoadPluginModules();
            compiler.LockReferences();
            compiler.LockBuildTarget();

            compiler.CompileAsync(ToSourceItems().ToImmutableArray()).Wait();

            foreach (var warning in compiler.Warnings)
                logger.Warn(warning);

            if (compiler.Errors.Any())
            {
                foreach (var error in compiler.Errors)
                    logger.Error(error);

                return false;
            }

            return true;
        }

        public void Watch(Logger logger, CancellationToken cancellationToken)
        {
            var watcher = new FileSystemWatcher(BaseDir!)
            {
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = "*",
                IncludeSubdirectories = true,
                EnableRaisingEvents = true
            };

            var compiler = new SharpXCompiler(ToCompilerOptions());
            compiler.LoadPluginModules();
            compiler.LockReferences();
            compiler.LockBuildTarget();

            var projectName = Path.GetFileName(Path.GetDirectoryName(BaseDir)) ?? "UNKNOWN";

            var handler = CreateThrottleEventHandler(TimeSpan.FromSeconds(1), (_, _) =>
            {
                logger.Info($"[{projectName}] File changed detected. Start compilation...");
                CompileAsync(logger, projectName, compiler, cancellationToken);
            });

            watcher.Created += handler;
            watcher.Changed += handler;
            watcher.Deleted += handler;

            _disposer = () =>
            {
                watcher.Created -= handler;
                watcher.Changed -= handler;
                watcher.Deleted -= handler;
                watcher.Dispose();
            };

            CompileAsync(logger, projectName, compiler, cancellationToken);
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


        private void CompileAsync(Logger logger, string projectName, SharpXCompiler compiler, CancellationToken cancellationToken)
        {
            try
            {
                compiler.CompileAsync(ToSourceItems().ToImmutableArray()).Wait(cancellationToken);

                logger.Info($"[{projectName}] Compilation finished with {compiler.Warnings.Count} warning(s) and {compiler.Errors.Count} error(s)");

                foreach (var warning in compiler.Warnings)
                    logger.Warn($"[{projectName}] {warning}");

                foreach (var error in compiler.Errors)
                    logger.Error($"[{projectName}] {error}");
            }
            catch (Exception e)
            {
                logger.Error(e, $"[{projectName}] Failed to compile");
            }
        }
    }
}