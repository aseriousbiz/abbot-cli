using System;
using System.CommandLine;
using System.Resources;
using System.Threading.Tasks;
using Serious.IO.CommandLine.Commands;

[assembly:CLSCompliant(false)]
[assembly:NeutralResourcesLanguage("en")]
namespace Serious.IO.CommandLine
{
    class Program
    {
#if DEBUG
        internal const string Website = "https://localhost:4979";
#else
        internal const string Website = "https://app.ab.bot";
#endif

        /// <summary>
        /// The main entry point
        /// </summary>
        /// <param name="args">The array of commandline arguments.</param>
        /// <returns>0 on success, an exit code on failure.</returns>
        public static async Task<int> Main(string[] args)
        {
            var context = Constructors.CreateCommandContext();

            var runCommand = new RunCommand(context);
            // Create a root command with some options
            var rootCommand = new RootCommand
            {
                new AuthCommand(context),
                new GetCommand(context),
                new InitCommand(context),
                new DeployCommand(context),
                new ListCommand(context),
                new ReplCommand(context, runCommand),
                runCommand,
                new StatusCommand(context)
            };

            rootCommand.Description = "Abbot command line. Use this to set up a local Abbot Workspace. To get started, run `abbot auth` in a directory where you want to edit skills.";

            // Parse the incoming args and invoke the handler
            return await rootCommand.InvokeAsync(args);
        }
    }
}
