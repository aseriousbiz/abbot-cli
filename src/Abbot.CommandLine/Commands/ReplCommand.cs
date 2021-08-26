using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace Serious.Abbot.CommandLine.Commands
{
    public class ReplCommand : Command
    {
        public ReplCommand() : base("repl", "Starts a REPL session for the specified skill. Hit CTRL+C to exit.")
        {
            Add(new Argument<string>("skill", () => string.Empty, "The name of the skill to run the REPL against"));
            var directoryOption = new Option<string>("--directory", "The Abbot Skills folder. If omitted, assumes the current directory.");
            directoryOption.AddAlias("-d");
            AddOption(directoryOption);
            Handler = CommandHandler.Create<string, string>(HandleReplCommandAsync);
        }

        static async Task<int> HandleReplCommandAsync(string skill, string directory)
        {
            Console.Clear();
            Console.Write($@"
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
                if (await RunCommand.HandleRunCommandAsync(skill, args ?? string.Empty, directory) != 0)
                {
                    break;
                }

                Console.Write($"$ @abbot {skill} ");
            }

            return 0;
        }
    }
}