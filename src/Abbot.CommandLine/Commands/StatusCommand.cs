using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Serious.Abbot.CommandLine.Services;

namespace Serious.Abbot.CommandLine.Commands
{
    public class StatusCommand : Command
    {
        readonly IWorkspaceFactory _workspaceFactory;

        public StatusCommand(IWorkspaceFactory workspaceFactory)
            : base("status", "Get the status of the Abbot Workspace.")
        {
            _workspaceFactory = workspaceFactory;
            this.AddDirectoryOption();
            Handler = CommandHandler.Create<string?>(HandleStatusCommandAsync);
        }

        public static string GetVersion()
        {
            return typeof(Program).Assembly.GetName().Version?.ToString() ?? "(unknown)";
        }

        async Task<int> HandleStatusCommandAsync(string? directory)
        {
            Console.WriteLine($"Running abbot-cli version {GetVersion()}");
            var workspace = _workspaceFactory.GetWorkspace(directory);
            var workingDir = workspace.WorkingDirectory.FullName;
            
            if (!workspace.WorkingDirectory.Exists)
            {
                Console.WriteLine(Messages.Directory_Does_Not_Exist, workingDir);
                return 0;
            }

            if (!workspace.IsInitialized)
            {
                var initDirectory = !workspace.DirectorySpecified
                    ? ""
                    : $" --directory {directory}";
                Console.WriteLine(Messages.Directory_Is_Not_Abbot_Development_Enviroment, workingDir, initDirectory);
                return 0;
            }

            if (!workspace.IsAuthenticated)
            {
                Console.WriteLine(Messages.Abbot_Directory_Not_Authenticated, workingDir, directory);
                return 0;
            }

            var response = await AbbotApi.CreateInstance(workspace).GetStatusAsync();
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
            
            var status = @$"The directory {workingDir} is an authenticated Workspace.
Organization: {organization.Name} ({organization.Platform} {organization.PlatformId})
User: {user.Name} ({user.PlatformUserId})";
            Console.WriteLine(status);
            return 0;
        }
    }
}