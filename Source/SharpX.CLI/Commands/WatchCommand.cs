using System;
using System.IO;
using System.Text.Json;
using System.Threading;

using NLog;

using SharpX.CLI.Models;

namespace SharpX.CLI.Commands
{
    internal class WatchCommand
    {
        private readonly WatchCommandArguments _args;
        private readonly Logger _logger;

        public WatchCommand(Logger logger, WatchCommandArguments args)
        {
            _logger = logger;
            _args = args;
        }

        public int Run()
        {
            if (string.IsNullOrWhiteSpace(_args.Project) && string.IsNullOrWhiteSpace(_args.Solution))
                throw new ArgumentException("project or solution is required");
            if (string.IsNullOrWhiteSpace(_args.Project))
                return WatchSolution(null);
            return WatchProject();
        }

        private int WatchProject()
        {
            var solution = new Solution("1", new[] { _args.Project });
            return WatchSolution(solution);
        }

        private int WatchSolution(Solution? providedSolution)
        {
            var solution = providedSolution ?? LoadSolution();
            var source = new CancellationTokenSource();
            var projects = solution.GetProjects();

            _logger.Info("Initial compilation...");

            foreach (var project in projects)
                project.Watch(_logger, source.Token);

            _logger.Info("Start watching projects and its source items...");

            Console.CancelKeyPress += (_, _) => source.Cancel();

            while (!source.IsCancellationRequested)
                Console.ReadKey(true);

            foreach (var project in projects)
                project.Dispose();

            return 0;
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