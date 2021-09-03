using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Threading.Tasks;

namespace Serious.IO.CommandLine.Commands
{
    /// <summary>
    /// Returns the status of an Abbot Workspace.
    /// </summary>
    public class StatusCommand : AbbotCommand
    {
        public StatusCommand(ICommandContext commandContext)
            : base(commandContext, "status", "Get the status of the Abbot Workspace.")
        {
            this.AddDirectoryOption();
            Handler = CommandHandler.Create<string?>(HandleStatusCommandAsync);
        }

        public static string GetVersion()
        {
            return typeof(Program).Assembly.GetName().Version?.ToString() ?? "(unknown)";
        }

        async Task<int> HandleStatusCommandAsync(string? directory)
        {
            Console.Out.WriteLine($"Running abbot-cli version {GetVersion()}");
            var workspace = GetWorkspace(directory);
            var workingDir = workspace.WorkingDirectory.FullName;
            
            if (!workspace.WorkingDirectory.Exists)
            {
                Console.Out.WriteLine(Messages.Directory_Does_Not_Exist, workingDir);
                return 0;
            }

            if (!workspace.IsInitialized)
            {
                var initDirectory = !workspace.DirectorySpecified
                    ? ""
                    : $" --directory {directory!}";
                Console.Out.WriteLine(Messages.Directory_Is_Not_Abbot_Development_Enviroment, workingDir, initDirectory);
                return 0;
            }

            if (!workspace.IsAuthenticated)
            {
                Console.Out.WriteLine(Messages.Abbot_Directory_Not_Authenticated, workingDir, directory ?? ".");
                return 0;
            }

            var response = await CreateApiClient(workspace).GetStatusAsync();
            if (!response.IsSuccessStatusCode)
            {
                if (!response.IsSuccessStatusCode)
                {
                    return await response.HandleUnsuccessfulResponseAsync();
                }
            }

            if (response.Content is null)
            {
                Console.Error.WriteLine("No response from Abbot.");
                return 1;
            }

            var organization = response.Content.Organization;
            var user = response.Content.User;
            
            var status = @$"The directory {workingDir} is an authenticated Workspace.
Organization: {organization.Name} ({organization.Platform} {organization.PlatformId})
User: {user.Name} ({user.PlatformUserId})";
            Console.Out.WriteLine(status);
            return 0;
        }
    }
}