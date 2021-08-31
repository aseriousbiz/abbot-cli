using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Threading.Tasks;
using Serious.Abbot.CommandLine.Services;
using Serious.Abbot.Messages;

namespace Serious.Abbot.CommandLine.Commands
{
    public class ListCommand : Command
    {
        readonly IWorkspaceFactory _workspaceFactory;
        
        public ListCommand(IWorkspaceFactory workspaceFactory)
            : base("list", "List (and optionally download) all the skills in your organization.")
        {
            _workspaceFactory = workspaceFactory;
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
            var workspace = _workspaceFactory.GetWorkspace(directory);
            if (!workspace.IsInitialized)
            {
                var directoryType = workspace.DirectorySpecified ? "specified" : "current";
                Console.WriteLine($"The {directoryType} directory is not an Abbot Workspace. Either specify the path to an Abbot Workspace, or initialize a new one using `abbot init`");
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
                    await GetCommand.CreateSkillWorkspaceAsync(skillInfo, workspace, force);
                }
            }
            else
            {
                foreach (var skill in response.Results.Select(skill => skill.Name))
                {
                    Console.WriteLine(skill);
                }
            }

            return 0;
        }

        static async Task<SkillListResponse?> GetSkillListAsync(
            Workspace workspace,
            SkillOrderBy orderBy,
            OrderDirection direction,
            bool includeDisabled)
        {
            var response = await AbbotApi.CreateInstance(workspace).ListSkillsAsync(orderBy, direction, includeDisabled);
            if (!response.IsSuccessStatusCode)
            {
                await response.HandleUnsuccessfulResponseAsync();
                return null;
            }

            if (response.Content is null)
            {
                await Console.Error.WriteLineAsync("Response content is null");
                return null;
            }

            return response.Content;
        }
    }
}