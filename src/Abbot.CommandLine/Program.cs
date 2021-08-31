using System;
using System.CommandLine;
using System.Resources;
using Serious.Abbot.CommandLine.Commands;
using Serious.Abbot.CommandLine.Services;

[assembly:CLSCompliant(false)]
[assembly:NeutralResourcesLanguage("en")]
namespace Serious.Abbot.CommandLine
{
    class Program
    {
#if DEBUG
        internal const string Website = "https://localhost:4979";
#else
        internal const string Website = "https://ab.bot";
#endif
        
        /// <summary>
        /// The main entry point
        /// </summary>
        /// <param name="args">The array of commandline arguments.</param>
        /// <returns>0 on success, an exit code on failure.</returns>
        static int Main(string[] args)
        {
            var factory = new WorkspaceFactory();
            var runCommand = new RunCommand(factory);
            // Create a root command with some options
            var rootCommand = new RootCommand
            {
                new AuthCommand(factory),
                new GetCommand(factory),
                new InitCommand(factory),
                new DeployCommand(factory),
                new ListCommand(factory),
                new ReplCommand(runCommand),
                runCommand,
                new StatusCommand(factory)
            };

            rootCommand.Description = "Abbot command line. Use this to set up a local Abbot Workspace. To get started, run `abbot auth` in a directory where you want to edit skills.";
            
            // Parse the incoming args and invoke the handler
            return rootCommand.InvokeAsync(args).Result;
        }
    }
}