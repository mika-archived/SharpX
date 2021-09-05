using System;
using System.IO;
using System.Linq;
using System.Text.Json;

using NLog;

using SharpX.CLI.Models;

namespace SharpX.CLI.Commands
{
    internal class BuildCommand
    {
        private readonly BuildCommandArguments _args;
        private readonly Logger _logger;

        public BuildCommand(Logger logger, BuildCommandArguments args)
        {
            _logger = logger;
            _args = args;
        }

        public int Run()
        {
            if (string.IsNullOrWhiteSpace(_args.Project) && string.IsNullOrWhiteSpace(_args.Solution))
                throw new ArgumentException("project or solution is required");
            if (string.IsNullOrWhiteSpace(_args.Project))
                return BuildSolution(null);
            return BuildProject();
        }

        private int BuildProject()
        {
            var solution = new Solution("1", new[] { _args.Project });
            return BuildSolution(solution);
        }

        private int BuildSolution(Solution? providedSolution)
        {
            var solution = providedSolution ?? LoadSolution();
            var projects = solution.GetProjects();
            var hasErrors = projects.Aggregate(false, (current, project) => current | !project.Build(_logger));

            foreach (var project in projects) 
                project.Dispose();

            return hasErrors ? -1 : 0;
        }

        private Solution LoadSolution()
        {
            if (!File.Exists(_args.Solution))
                throw new FileNotFoundException(null, _args.Solution);

            var solution = JsonSerializer.Deserialize<Solution>(File.ReadAllText(_args.Solution));
            if (solution == null)
                throw new ArgumentException();

            return solution with { BaseDirectory = Path.GetDirectoryName(_args.Solution) };
        }
    }
}