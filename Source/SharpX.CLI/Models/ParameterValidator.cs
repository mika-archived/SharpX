using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

using NLog;

namespace SharpX.CLI.Models
{
    public static class ParameterValidator
    {
        public static bool ValidateOptions(Logger logger, string project)
        {
            if (string.IsNullOrWhiteSpace(project))
                return false;
            return ValidateProjectOptions(logger, project);
        }

        public static CompilerConfiguration CreateConfiguration(string project)
        {
            if (string.IsNullOrWhiteSpace(project))
                throw new InvalidOperationException();

            try
            {
                var obj = JsonSerializer.Deserialize<CompilerConfiguration>(File.ReadAllText(project));
                var root = Path.GetDirectoryName(project)!;

                return new CompilerConfiguration(Path.Combine(root, obj.BaseDir), obj.Includes, obj.Excludes ?? Array.Empty<string>(), obj.References.Select(w => Path.Combine(root, w)).ToArray(), obj.Plugins.Select(w => Path.Combine(root, w)).ToArray(), Path.Combine(root, obj.Out), obj.Target) with
                {
                    CustomOptions = obj.CustomOptions
                };
            }
            catch (Exception e)
            {
                throw new InvalidOperationException();
            }
        }

        private static bool ValidateProjectOptions(Logger logger, string project)
        {
            if (!File.Exists(project))
                return false;

            try
            {
                var obj = JsonSerializer.Deserialize<CompilerConfiguration>(File.ReadAllText(project));
                if (obj == null)
                    return false;

                var root = Path.GetDirectoryName(project)!;

                return ValidateRawOptions(logger, Path.Combine(root, obj.BaseDir), obj.Includes, obj.Excludes, Path.Combine(root, obj.Out), obj.References.Select(w => Path.Combine(root, w)).ToArray(), obj.Plugins.Select(w => Path.Combine(root, w)).ToArray(), obj.Target, obj.CustomOptions);
            }
            catch (Exception e)
            {
                logger.Error(e, "Failed to load or parse project configuration");
                return false;
            }
        }

        private static bool ValidateRawOptions(Logger logger, string? baseDir, string[]? includes, string[]? excludes, string? @out, string[]? references, string[]? plugins, string? target, Dictionary<string, JsonElement>? extras)
        {
            if (string.IsNullOrWhiteSpace(baseDir) || !Directory.Exists(baseDir))
            {
                logger.Error("invalid base directory: directory is empty or not exists.");
                return false;
            }

            if (includes == null || includes.Length == 0)
            {
                logger.Error("invalid includes: includes must be one or greater than items.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(@out))
            {
                logger.Error("invalid out directory: directory is empty");
                return false;
            }

            if (references is { Length: > 0 } && references.Any(w => !File.Exists(w)))
            {
                logger.Error("invalid references: one or more reference dll is not exists");
                return false;
            }

            if (plugins is { Length: > 0 } && plugins.Any(w => !File.Exists(w)))
            {
                logger.Error("invalid plugins: one or more plugin dll is not exists");
                return false;
            }

            if (string.IsNullOrWhiteSpace(target))
            {
                logger.Error("invalid target: target is empty");
                return false;
            }

            return true;
        }
    }
}