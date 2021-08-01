using System.Threading.Tasks;

using ConsoleAppFramework;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using ShaderSharp.CLI.Commands;

namespace ShaderSharp.CLI
{
    internal class CompilerInterface : ConsoleAppBase
    {
        private readonly ILogger<CompilerInterface> _logger;

        public CompilerInterface(ILogger<CompilerInterface> logger)
        {
            _logger = logger;
        }

        private static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder().RunConsoleAppFrameworkAsync<CompilerInterface>(args);
        }

        [Command("init")]
        public int Init([Option(0)] string path = null)
        {
            return new InitCommand(_logger, path).Run();
        }
    }
}