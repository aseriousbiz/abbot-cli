using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Net;
using System.Threading.Tasks;
using Serious.Abbot.CommandLine.Services;

namespace Serious.Abbot.CommandLine.Commands
{
    public class StatusCommand : Command
    {
        public StatusCommand() : base("status", "Get the status of the Abbot development directory.")
        {
            Add(new Argument<string>("directory", () => ".", "The directory to set up as a local Abbot development environment. This will create an `.abbot` folder in that directory. If the directory does not exist, this creates the directory."));
            Handler = CommandHandler.Create<string>(HandleStatusCommandAsync);
        }

        static async Task<int> HandleStatusCommandAsync(string directory)
        {
            var environment = DevelopmentEnvironment.GetEnvironment(directory);
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
                var message = response.StatusCode switch
                {
                    HttpStatusCode.InternalServerError => "An error occurred on the server. Contact support@aseriousbusiness.com to learn more. It's their fault.",
                    HttpStatusCode.Unauthorized => "The API Key you provided is not valid or expired. Run \"abbot auth\" to authenticate again.",
                    _ => $"Received a {response.StatusCode} response from the server"
                };
                await Console.Error.WriteLineAsync(message);
                return 1;
            }

            if (response.Content is null)
            {
                await Console.Error.WriteLineAsync("No response from Abbot.");
                return 1;
            }

            var organization = response.Content.Organization;
            var user = response.Content.User;
            
            var status = @$"The directory {workingDir} is an authenticated Abbot Skill Development environment.
Organization: {organization.Name} ({organization.Platform} {organization.PlatformId})
User: {user.Name} ({user.PlatformUserId})";
            Console.WriteLine(status);
            return 0;
        }
    }
}