using System;

using NLog;
using NLog.Config;
using NLog.Targets;

using SharpX.CLI.Commands;

namespace SharpX.CLI
{
    public class CompilerInterface
    {
        private static int Main(string[] args)
        {
            if (args.Length == 0)
                return -1;

            var executor = args[0];

            var logger = ConfigureLogger();

            switch (executor)
            {
                case "init":
                    return Init(logger, args[1..]);

                case "build":
                    return Build(logger, args[1..]);

                case "watch":
                    return Watch(logger, args[1..]);

                default:
                    throw new InvalidOperationException($"{executor} is unavailable command");
            }
        }

        private static Logger ConfigureLogger()
        {
            var noErrorConsole = new ColoredConsoleTarget
            {
                Name = "ColoredConsole",
                Layout = "${pad:padding=5:inner=${level}}: ${message}",
                UseDefaultRowHighlightingRules = false,
                WordHighlightingRules =
                {
                    new ConsoleWordHighlightingRule(LogLevel.Info.ToString(), ConsoleOutputColor.Green, ConsoleOutputColor.NoChange),
                    new ConsoleWordHighlightingRule(LogLevel.Warn.ToString(), ConsoleOutputColor.Yellow, ConsoleOutputColor.NoChange),
                }
            };

            var errorConsole = new ColoredConsoleTarget
            {
                Name = "ColoredErrorConsole",
                Layout = "${pad:padding=5:inner=${level}}: ${message}${newline}${exception:format=Message, Type, ToString:separator=*}",
                UseDefaultRowHighlightingRules = false,
                WordHighlightingRules =
                {
                    new ConsoleWordHighlightingRule(LogLevel.Error.ToString(), ConsoleOutputColor.Black, ConsoleOutputColor.Red),
                }
            };

            var config = new LoggingConfiguration();
            config.AddRuleForOneLevel(LogLevel.Debug, noErrorConsole);
            config.AddRuleForOneLevel(LogLevel.Warn, noErrorConsole);
            config.AddRuleForOneLevel(LogLevel.Info, noErrorConsole);
            config.AddRuleForOneLevel(LogLevel.Error, errorConsole);

            LogManager.Configuration = config;
            return LogManager.GetCurrentClassLogger();
        }

        private static int Init(Logger logger, string[] args)
        {
            var path = args.Length > 0 ? args[0] : null;
            return new InitCommand(logger, path).Run();
        }

        private static int Build(Logger logger, string[] args)
        {
            if (args.Length != 1)
                return -1;

            var project = args[0];
            return new BuildCommand(logger, project).Run();
        }

        private static int Watch(Logger logger, string[] args)
        {
            if (args.Length != 1)
                return -1;

            var project = args[0];
            return new WatchCommand(logger, project).Run();
        }
    }
}