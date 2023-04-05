using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Serious.IO.CommandLine.Services;

namespace Serious.IO.CommandLine.Commands
{
    public class ExportCommand : AbbotCommand
    {
        public ExportCommand(ICommandContext commandContext)
            : base(commandContext, "export", "Export insights data to the console.")
        {
            this.AddDirectoryOption();
            Handler = CommandHandler.Create<string>(HandleExportCommandAsync);
        }

        async Task<int> HandleExportCommandAsync(string directory)
        {
            var workspace = GetWorkspace(directory);
            if (!workspace.IsInitialized)
            {
                var directoryType = workspace.DirectorySpecified ? "specified" : "current";
                Console.Out.WriteLine($"The {directoryType} directory is not an Abbot Workspace. Either specify the path to an Abbot Workspace, or initialize a new one using `abbot init`");
                return 1;
            }

            var response = await GetInsightsExportAsync(workspace);
            if (response is null)
            {
                return 1;
            }

            var export = JsonSerializer.Serialize(response);
            Console.Out.Write(export);

            return 0;
        }

        async Task<object?> GetInsightsExportAsync(Workspace workspace)
        {
            var response = await CreateApiClient(workspace)
                .GetInsightsExportAsync();
            if (!response.IsSuccessStatusCode)
            {
                await response.HandleUnsuccessfulResponseAsync();
                return null;
            }

            if (response.Content is null)
            {
                Console.Error.WriteLine("Response content is null");
                return null;
            }

            return response.Content;
        }
    }
}
