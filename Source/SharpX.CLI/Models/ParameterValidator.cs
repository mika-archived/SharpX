using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

using Microsoft.Extensions.Logging;

namespace SharpX.CLI.Models
{
    public static class ParameterValidator
    {
        public static bool ValidateOptions(ILogger logger, string? project, string? baseDir, string[]? includes, string[]? excludes, string? @out, string[]? references, string[]? plugins, string? target)
        {
            if (string.IsNullOrWhiteSpace(project))
                return ValidateRawOptions(logger, baseDir, includes, excludes, @out, references, plugins, target, null);
            return ValidateProjectOptions(logger, project);
        }

        public static CompilerConfiguration CreateConfiguration(string? project, string? baseDir, string[]? includes, string[]? excludes, string? @out, string[]? references, string[]? plugins, string? target, Dictionary<string, JsonElement>? extras)
        {
            if (string.IsNullOrWhiteSpace(project))
                return new CompilerConfiguration(baseDir!, includes!, excludes ?? Array.Empty<string>(), references ?? Array.Empty<string>(), plugins ?? Array.Empty<string>(), @out!, target!) { CustomOptions = extras ?? new Dictionary<string, JsonElement>() };

            try
            {
                var obj = JsonSerializer.Deserialize<CompilerConfiguration>(File.ReadAllText(project));
                var root = Path.GetDirectoryName(project)!;

                return CreateConfiguration(null, Path.Combine(root, obj.BaseDir), obj.Includes, obj.Excludes, Path.Combine(root, obj.Out), obj.References.Select(w => Path.Combine(root, w)).ToArray(), obj.Plugins.Select(w => Path.Combine(root, w)).ToArray(), obj.Target, obj.CustomOptions);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException();
            }
        }

        private static bool ValidateProjectOptions(ILogger logger, string project)
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
                logger.LogError(e, "Failed to load or parse project configuration");
                return false;
            }
        }

        private static bool ValidateRawOptions(ILogger logger, string? baseDir, string[]? includes, string[]? excludes, string? @out, string[]? references, string[]? plugins, string? target, Dictionary<string, JsonElement>? extras)
        {
            if (string.IsNullOrWhiteSpace(baseDir) || !Directory.Exists(baseDir))
            {
                logger.LogError("invalid base directory: directory is empty or not exists.");
                return false;
            }

            if (includes == null || includes.Length == 0)
            {
                logger.LogError("invalid includes: includes must be one or greater than items.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(@out))
            {
                logger.LogError("invalid out directory: directory is empty");
                return false;
            }

            if (references is { Length: > 0 } && references.Any(w => !File.Exists(w)))
            {
                logger.LogError("invalid references: one or more reference dll is not exists");
                foreach (var missing in references.Where(w => !File.Exists(w)))
                    logger.LogError($"missing file: {missing}");
                return false;
            }

            if (plugins is { Length: > 0 } && plugins.Any(w => !File.Exists(w)))
            {
                logger.LogError("invalid plugins: one or more plugin dll is not exists");
                return false;
            }

            if (string.IsNullOrWhiteSpace(target))
            {
                logger.LogError("invalid target: target is empty");
                return false;
            }

            return true;
        }
    }
}