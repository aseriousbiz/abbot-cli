using System;
using System.CommandLine;
using System.Resources;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Serious.IO.CommandLine.Commands;
using Serious.IO.CommandLine.Services;
using Serious.Secrets;

[assembly:CLSCompliant(false)]
[assembly:NeutralResourcesLanguage("en")]
namespace Serious.IO.CommandLine
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
        public static async Task<int> Main(string[] args)
        {
            var fileSystem = new FileSystem();
            var dataProtector = DataProtectionProvider
                .Create("abbot")
                .CreateProtector("Store Abbot Workspace Secrets");
            var secretProtector = new SecretProtector(dataProtector);
            var apiClientFactory = new ApiClientFactory();
            var console = new ExtendedSystemConsole();
            var tokenConstructor =
                Constructors.CreateTokenStoreConstructor(
                    Constructors.CreateSecretStoreConstructor(fileSystem, secretProtector));
            var workspaceConstructor = Constructors.WorkspaceConstructor();
            var workspaceFactory = new WorkspaceFactory(fileSystem, tokenConstructor, workspaceConstructor);
            
            var context = new CommandContext(console, apiClientFactory, workspaceFactory);
            
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