using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;
using Serious.Abbot.CommandLine.Editors;
using Serious.Abbot.CommandLine.Services;
using Serious.Abbot.Messages;

namespace Serious.Abbot.CommandLine.Commands
{
    public class RunCommand : Command
    {
        public RunCommand() : base("run", "Runs your skill code in the skill runner")
        {
            Add(new Argument<string>("skill", () => string.Empty, "The name of the skill to run"));
            Add(new Argument<string>("arguments", () => ".", "The arguments to pass to the skill"));
            var directoryOption = new Option<string>("--directory", "The Abbot Skills folder. If omitted, assumes the current directory.");
            directoryOption.AddAlias("-d");
            AddOption(directoryOption);

            Handler = CommandHandler.Create<string, string, string?>(HandleRunCommandAsync);
        }

        internal static async Task<int> HandleRunCommandAsync(string skill, string arguments, string? directory)
        {
            var environment = DevelopmentEnvironment.GetEnvironment(directory ?? ".");
            if (!environment.IsInitialized)
            {
                var directoryType = directory == "." ? "current" : "specified";
                Console.WriteLine($"The {directoryType} directory is not an Abbot Skills folder. Either specify the directory where you've initialized an environment, or initialize a new one using `abbot init`");
                return 1;
            }

            var skillEnvironment = environment.GetSkillEnvironment(skill);
            
            if (!skillEnvironment.Exists)
            {
                Console.WriteLine($"The directory {skillEnvironment.WorkingDirectory} does not exist. Have you run `abbot download {skill}` yet?");
                return 1;
            }

            var codeFile = skillEnvironment.GetCodeFilePath();
            if (codeFile is null)
            {
                Console.WriteLine($"Did not find a code file in {skillEnvironment.WorkingDirectory}");
                return 1;
            }
            
            var code = await File.ReadAllTextAsync(codeFile);
            code = Omnisharp.RemoveGlobalsDirective(code);
            
            var runRequest = new SkillRunRequest
            {
                Code = code,
                Arguments = arguments,
                Name = skill
            };

            var response = await AbbotApi.CreateInstance(environment)
                .RunSkillAsync(skill, runRequest);
            
            if (!response.IsSuccessStatusCode)
            {
                return await response.HandleUnsuccessfulResponseAsync();
            }

            if (response.Content?.Replies is null)
            {
                await Console.Error.WriteLineAsync("Response content is null");
                return 1;
            }

            Console.WriteLine(string.Join("\n", response.Content.Replies));
            return 0;
        }
    }
}