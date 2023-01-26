using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Linq;
using System.Threading.Tasks;
using Serious.Abbot.Messages;
using Serious.IO.CommandLine.Services;

namespace Serious.IO.CommandLine.Commands
{
    public class ListCommand : AbbotCommand
    {
        public ListCommand(ICommandContext commandContext)
            : base(commandContext, "list", "List (and optionally download) all the skills in your organization.")
        {
            this.AddDirectoryOption();
            this.AddOption<bool>(
                "--get",
                "-c",
                "If specified, retrieves every skill as if running `abbot get` on every skill.");
            this.AddOption<bool>(
                "--force",
                "-f",
                "If true, overwrites the local skill code if it exists even if it has changes. Ignored if --get not set.");
            this.AddOption<bool>(
                "--include-disabled",
                "-id",
                "If specified, includes disabled skills.");
            this.AddOption<SkillOrderBy>(
                "--order-by",
                "-o",
                "Order skills by \"Name\", \"Created\", or \"Modified\"");
            this.AddOption<OrderDirection>(
                "--direction",
                "-od",
                "Order direction. Either \"Ascending\" or \"Descending\"");
            Handler = CommandHandler.Create<string, bool, bool, bool, SkillOrderBy, OrderDirection>(HandleListCommandAsync);
        }

        async Task<int> HandleListCommandAsync(string? directory, bool get, bool force, bool includeDisabled, SkillOrderBy orderBy, OrderDirection direction)
        {
            var workspace = GetWorkspace(directory);
            if (!workspace.IsInitialized)
            {
                var directoryType = workspace.DirectorySpecified ? "specified" : "current";
                Console.Out.WriteLine($"The {directoryType} directory is not an Abbot Workspace. Either specify the path to an Abbot Workspace, or initialize a new one using `abbot init`");
                return 1;
            }

            var response = await GetSkillListAsync(workspace, orderBy, direction, includeDisabled);
            if (response is null)
            {
                return 1;
            }

            if (get)
            {
                foreach (var skillInfo in response.Results)
                {
                    await GetCommand.CreateSkillWorkspaceAsync(skillInfo, workspace, Console, force);
                }
            }
            else
            {
                foreach (var skill in response.Results.Select(skill => skill.Name))
                {
                    Console.Out.WriteLine(skill);
                }
            }

            return 0;
        }

        async Task<SkillListResponse?> GetSkillListAsync(
            Workspace workspace,
            SkillOrderBy orderBy,
            OrderDirection direction,
            bool includeDisabled)
        {
            var response = await CreateApiClient(workspace)
                .ListSkillsAsync(orderBy, direction, includeDisabled);
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
