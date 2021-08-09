using System;
using System.IO;
using System.Text.Json;

using Microsoft.Extensions.Logging;

using SharpX.CLI.Models;

namespace SharpX.CLI.Commands
{
    internal class InitCommand
    {
        private readonly ILogger<CompilerInterface> _logger;
        private readonly string _path;

        public InitCommand(ILogger<CompilerInterface> logger, string path)
        {
            _logger = logger;
            _path = path ?? Directory.GetCurrentDirectory();
        }

        public int Run()
        {
            try
            {
                var defaultConfig = new CompilerConfiguration
                {
                    Sources = new[] { "**/*.cs" },
                    References = new string[] { },
                    Out = "dist"
                };

                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(defaultConfig, options);

                File.WriteAllText(Path.Combine(_path, "ssconfig.json"), json);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to generate ssconfig.json");
                return 1;
            }

            return 0;
        }
    }
}