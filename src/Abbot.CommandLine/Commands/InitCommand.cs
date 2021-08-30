using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Serious.Abbot.CommandLine.Services;

namespace Serious.Abbot.CommandLine.Commands
{
    public class InitCommand : Command
    {
        readonly IWorkspaceFactory _workspaceFactory;

        public InitCommand(IWorkspaceFactory workspaceFactory)
            : base("init", "Set up a directory as an Abbot Workspace")
        {
            _workspaceFactory = workspaceFactory;
            this.AddDirectoryOption();
            Handler = CommandHandler.Create<string?>(HandleInitCommandAsync);
        }

        async Task<int> HandleInitCommandAsync(string? directory)
        {
            var workspace = _workspaceFactory.GetWorkspace(directory);
            await workspace.EnsureAsync();

            Console.WriteLine(Messages.Initialized_Abbot_Directory, workspace.WorkingDirectory);
            return 0;
        }
    }
}