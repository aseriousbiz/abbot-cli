using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Threading.Tasks;

namespace Serious.Abbot.CommandLine.Commands
{
    public class ReplCommand : AbbotCommand
    {
        readonly RunCommand _runCommand;

        public ReplCommand(ICommandContext commandContext, RunCommand runCommand)
            : base(commandContext, "repl", "Starts a REPL session for the specified skill. Hit CTRL+C to exit.")
        {
            _runCommand = runCommand;
            Add(new Argument<string>("skill", () => string.Empty, "The name of the skill to run the REPL against"));
            this.AddDirectoryOption();
            this.AddOption<bool>("--deployed", "-s", "If specified, runs the deployed version of the skill. Aka, the version on the server.");
            Handler = CommandHandler.Create<string, string?, bool>(HandleReplCommandAsync);
        }

        async Task<int> HandleReplCommandAsync(string skill, string? directory, bool deployed)
        {
            var workspace = GetWorkspace(directory);
            
            if (!workspace.IsInitialized)
            {
                return HandleUninitializedWorkspace(workspace);
            }

            var code = await _runCommand.GetCodeToRunAsync(skill, workspace, deployed);
            if (code is null)
            {
                return 1;
            }
            
            Console.Clear();
            Console.Out.Write($@"
           _     _           _    
          | |   | |         | |   
      __ _| |__ | |__   ___ | |_  
     / _` | '_ \| '_ \ / _ \| __| 
    | (_| | |_) | |_) | (_) | |_  
     \__,_|_.__/|_.__/ \___/ \__| 

Abbot {StatusCommand.GetVersion()}.

Welcome to the Abbot REPL for the ""{skill}"" skill!
Enter arguments to the skill and hit <ENTER> to run the local version of the skill.

$ @abbot {skill} ");
            while (true)
            {
                var args = Console.ReadLine();
                if (await _runCommand.RunCodeAsync(skill, args ?? string.Empty, code, workspace, deployed) != 0)
                {
                    break;
                }

                Console.Out.Write($"$ @abbot {skill} ");
            }

            return 0;
        }
    }
}