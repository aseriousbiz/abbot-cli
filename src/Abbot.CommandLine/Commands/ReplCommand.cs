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
            Add(new Argument<string>("directory", () => ".", "The Abbot development directory. If omitted, uses the current directory."));
            Handler = CommandHandler.Create<string, string>(HandleReplCommandAsync);
        }

        static async Task<int> HandleReplCommandAsync(string skill, string directory)
        {
            Console.Clear();
            Console.Write($@"
   _____ ___.  ___.           __   
  /  _  \\_ |__\_ |__   _____/  |_ 
 /  /_\  \| __ \| __ \ /  _ \   __\
/    |    \ \_\ \ \_\ (  <_> )  |  
\____|__  /___  /___  /\____/|__|  
        \/    \/    \/             

Welcome to the Abbot REPL for the ""{skill}"" skill!
You can test your skill by entering arguments to the skill and hitting <ENTER>.

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