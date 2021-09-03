using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace Serious.IO.CommandLine.Commands
{
    public class InitCommand : AbbotCommand
    {
        public InitCommand(ICommandContext commandContext)
            : base(commandContext, "init", "Set up a directory as an Abbot Workspace")
        {
            this.AddDirectoryOption();
            Handler = CommandHandler.Create<string?>(HandleInitCommandAsync);
        }

        async Task<int> HandleInitCommandAsync(string? directory)
        {
            var workspace = GetWorkspace(directory);
            await workspace.EnsureAsync();

            Console.Out.WriteLine(Messages.Initialized_Abbot_Directory, workspace.WorkingDirectory);
            return 0;
        }
    }
}