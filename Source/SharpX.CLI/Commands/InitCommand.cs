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

        public InitCommand(Logger logger, InitCommandArguments args)
        {
            _logger = logger;
            _path = args.Path ?? Directory.GetCurrentDirectory();
        }

        public int Run()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };

                var project = new Project("1", "./", new[] { "**/*.cs" }, Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), "dist", "None");
                var projectJson = JsonSerializer.Serialize(project, options);

                File.WriteAllText(Path.Combine(_path, "sxc.config.json"), projectJson);

                var solution = new Solution("1", new[] { "./sxc.config.json" });
                var solutionJson = JsonSerializer.Serialize(solution, options);

                File.WriteAllText(Path.Combine(_path, "sxc.sol.json"), solutionJson);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Failed to generate sxc.config.json and sxc.sol.json");
                return 1;
            }

            return 0;
        }
    }
}