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
            this.AddOption<string>("--secrets-directory", "-sd", $"Specifies an alternative directory to use for secrets.");
            Handler = CommandHandler.Create<string?, string?>(HandleInitCommandAsync);
        }

        async Task<int> HandleInitCommandAsync(string? directory, string? secretsDirectory)
        {
            var workspace = GetWorkspace(directory);
            await workspace.EnsureAsync(secretsDirectory);

            Console.Out.WriteLine(Messages.Initialized_Abbot_Directory, workspace.WorkingDirectory);
            return 0;
        }
    }
}