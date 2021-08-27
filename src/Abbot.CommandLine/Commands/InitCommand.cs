using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Serious.Abbot.CommandLine.Services;

namespace Serious.Abbot.CommandLine.Commands
{
    public class InitCommand : Command
    {
        readonly IDevelopmentEnvironmentFactory _developmentEnvironmentFactory;

        public InitCommand(IDevelopmentEnvironmentFactory developmentEnvironmentFactory)
            : base("init", "Set up a local Abbot development directory")
        {
            _developmentEnvironmentFactory = developmentEnvironmentFactory;
            var directoryOption = new Option<string>("--directory", "The directory to set up as a local Abbot development environment. This will create a `.abbot` folder in that directory. If the directory does not exist, this creates the directory.");
            directoryOption.AddAlias("-d");
            AddOption(directoryOption);
            Handler = CommandHandler.Create<string>(HandleInitCommandAsync);
        }

        async Task<int> HandleInitCommandAsync(string directory)
        {
            var environment = _developmentEnvironmentFactory.GetDevelopmentEnvironment(directory);
            await environment.EnsureAsync();

            Console.WriteLine(Messages.Initialized_Abbot_Directory, environment.WorkingDirectory);
            return 0;
        }
    }
}