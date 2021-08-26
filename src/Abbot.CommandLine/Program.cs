using System;
using System.CommandLine;
using System.Resources;
using Serious.Abbot.CommandLine.Commands;

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
            // Create a root command with some options
            var rootCommand = new RootCommand
            {
                new AuthCommand(),
                new GetCommand(),
                new InitCommand(),
                new DeployCommand(),
                new ReplCommand(),
                new RunCommand(),
                new StatusCommand()
            };

            rootCommand.Description = "Abbot command line. Use this to set up a local Abbot editing environment. To get started, run `abbot init .` in a directory where you want to edit skills.";
            
            // Parse the incoming args and invoke the handler
            return rootCommand.InvokeAsync(args).Result;
        }
    }
}