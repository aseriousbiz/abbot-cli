using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Serious.IO.CommandLine.Commands
{
    public class AuthCommand : AbbotCommand
    {
        const string TokenPage = $"{Program.Website}/account/apikeys";

        public AuthCommand(ICommandContext commandContext)
            : base(commandContext, "auth", "Authenticate the abbot command line")
        {
            this.AddDirectoryOption();
            this.AddOption<string>("--token", "-t", $"The API Key token created at {TokenPage}.");
            Handler = CommandHandler.Create<string?, string>(HandleAuthenticateCommandAsync);
        }
        
        /// <summary>
        /// Initiates authentication by launching the browser to the tokens page.
        /// </summary>
        async Task<int> HandleAuthenticateCommandAsync(string? directory, string token)
        {
            var workspace = GetWorkspace(directory);
            await workspace.EnsureAsync();
            
            if (token is { Length: > 0 })
            {
                await workspace.SetTokenAsync(token);
                return 0;
            }

            Console.Out.WriteLine(Messages.Auth_Message, TokenPage);
            
            var psi = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = TokenPage
            };
            Process.Start(psi);

            Console.Out.Write("Type in the API Key token and hit ENTER: ");
            var readToken = Console.ReadLine();
            if (readToken is { Length: > 0 })
            {
                await workspace.SetTokenAsync(readToken);
                return 0;
            }
            return 0;
        }
    }
}