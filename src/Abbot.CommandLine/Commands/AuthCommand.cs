using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Threading.Tasks;
using Serious.Abbot.CommandLine.Services;

namespace Serious.Abbot.CommandLine.Commands
{
    public class AuthCommand : Command
    {
        const string TokenPage = $"{Program.Website}/account/apikeys";

        public AuthCommand() : base("auth", "Authenticate the abbot command line")
        {
            var directoryOption = new Option<string>("--directory", "The directory to authenticate as a local Abbot development environment. If the directory is not an Abbot development environment, this will set it up as one and then authenticate.");
            directoryOption.AddAlias("-d");
            AddOption(directoryOption);
            var tokenOption = new Option<string>("--token", $"The API Key token created at {TokenPage}.");
            tokenOption.AddAlias("-t");
            AddOption(tokenOption);
            Handler = CommandHandler.Create<string, string>(HandleAuthenticateCommandAsync);
        }
        
        /// <summary>
        /// Initiates authentication by launching the browser to the tokens page.
        /// </summary>
        static async Task<int> HandleAuthenticateCommandAsync(string directory, string token)
        {
            var environment = await DevelopmentEnvironment.EnsureEnvironmentAsync(directory);
            if (token is { Length: > 0 })
            {
                await environment.SetTokenAsync(token);
                return 0;
            }

            Console.WriteLine(Messages.Auth_Message, TokenPage);
            
            var psi = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = TokenPage
            };
            Process.Start(psi);

            Console.Write("Type in the API Key token and hit ENTER: ");
            var readToken = Console.ReadLine();
            if (readToken is { Length: > 0 })
            {
                await environment.SetTokenAsync(readToken);
                return 0;
            }
            return 0;
        }
    }
}