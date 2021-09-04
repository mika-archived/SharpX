using System;
using System.IO;
using System.Text.Json;

using NLog;

using SharpX.CLI.Models;

namespace SharpX.CLI.Commands
{
    internal class InitCommand
    {
        private readonly Logger _logger;
        private readonly string _path;

        public InitCommand(Logger logger, string? path)
        {
            _logger = logger;
            _path = path ?? Directory.GetCurrentDirectory();
        }

        public int Run()
        {
            try
            {
                var defaultConfig = new CompilerConfiguration("", new[] { "**/*.cs" }, Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), "dist", "None");
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(defaultConfig, options);

                File.WriteAllText(Path.Combine(_path, "sxc.config.json"), json);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Failed to generate sxc.config.json");
                return 1;
            }

            return 0;
        }
    }
}