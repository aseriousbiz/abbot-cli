using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Serious.Abbot.CommandLine.Services;

namespace Serious.Abbot.CommandLine.Commands
{
    public class InitCommand : Command
    {
        public InitCommand() : base("init", "Set up a local Abbot development directory")
        {
            Add(new Argument<string>("directory", () => ".", "The directory to set up as a local Abbot development environment. This will create an `.abbot` folder in that directory. If the directory does not exist, this creates the directory."));
            Handler = CommandHandler.Create<string>(HandleInitCommandAsync);
        }

        static async Task<int> HandleInitCommandAsync(string directory)
        {
            var dir = await DevelopmentEnvironment.EnsureEnvironmentAsync(directory);

            Console.WriteLine(Messages.Initialized_Abbot_Directory, dir.WorkingDirectory.FullName);
            return 0;
        }
    }
}