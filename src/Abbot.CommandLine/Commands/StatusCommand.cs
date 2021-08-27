using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Serious.Abbot.CommandLine.Services;

namespace Serious.Abbot.CommandLine.Commands
{
    public class StatusCommand : Command
    {
        readonly IDevelopmentEnvironmentFactory _developmentEnvironmentFactory;

        public StatusCommand(IDevelopmentEnvironmentFactory developmentEnvironmentFactory)
            : base("status", "Get the status of the Abbot development directory.")
        {
            _developmentEnvironmentFactory = developmentEnvironmentFactory;
            var directoryOption = new Option<string>("--directory", "The Abbot Skills folder. If omitted, assumes the current directory.");
            directoryOption.AddAlias("-d");
            AddOption(directoryOption);
            Handler = CommandHandler.Create<string>(HandleStatusCommandAsync);
        }

        public static string GetVersion()
        {
            return typeof(Program).Assembly.GetName().Version?.ToString() ?? "(unknown)";
        }

        async Task<int> HandleStatusCommandAsync(string directory)
        {
            Console.WriteLine($"Running abbot-cli version {GetVersion()}");
            var environment = _developmentEnvironmentFactory.GetDevelopmentEnvironment(directory);
            var workingDir = environment.WorkingDirectory.FullName;
            
            if (!environment.WorkingDirectory.Exists)
            {
                Console.WriteLine(Messages.Directory_Does_Not_Exist, workingDir);
                return 0;
            }

            if (!environment.IsInitialized)
            {
                Console.WriteLine(Messages.Directory_Is_Not_Abbot_Development_Enviroment, workingDir, directory);
                return 0;
            }

            if (!environment.IsAuthenticated)
            {
                Console.WriteLine(Messages.Abbot_Directory_Not_Authenticated, workingDir, directory);
                return 0;
            }

            var response = await AbbotApi.CreateInstance(environment).GetStatusAsync();
            if (!response.IsSuccessStatusCode)
            {
                if (!response.IsSuccessStatusCode)
                {
                    return await response.HandleUnsuccessfulResponseAsync();
                }
            }

            if (response.Content is null)
            {
                await Console.Error.WriteLineAsync("No response from Abbot.");
                return 1;
            }

            var organization = response.Content.Organization;
            var user = response.Content.User;
            
            var status = @$"The directory {workingDir} is an authenticated Abbot Skills folder.
Organization: {organization.Name} ({organization.Platform} {organization.PlatformId})
User: {user.Name} ({user.PlatformUserId})";
            Console.WriteLine(status);
            return 0;
        }
    }
}