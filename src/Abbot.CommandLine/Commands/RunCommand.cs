using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
            var option = new Option<string>("--directory", "The Abbot Skills Development Environment directory. If omitted, assumes the current directory.");
            option.AddAlias("-d");
            AddOption(option);
            Handler = CommandHandler.Create<string, string, string?>(HandleRunCommandAsync);
        }

        static async Task<int> HandleRunCommandAsync(string skill, string arguments, string? directory)
        {
            var environment = DevelopmentEnvironment.GetEnvironment(directory ?? ".");
            if (!environment.IsInitialized)
            {
                var directoryType = directory == "." ? "current" : "specified";
                Console.WriteLine($"The {directoryType} directory is not an Abbot Skills Development Environment. Either specify the directory where you've initialized an environment, or initialize a new one using `abbot init`");
                return 1;
            }
            
            var skillDirectory = new DirectoryInfo(Path.Combine(environment.WorkingDirectory.FullName, skill));
            if (!skillDirectory.Exists)
            {
                Console.WriteLine($"The directory {skillDirectory.FullName} does not exist. Have you run `abbot download {skill}` yet?");
                return 1;
            }

            var codeFiles = skillDirectory.GetFiles($"{skill}.*");
            if (codeFiles is { Length: 0 })
            {
                Console.WriteLine($"Did not find a code file in {skillDirectory.FullName}");
                return 1;
            }

            if (codeFiles is { Length: > 1 })
            {
                Console.WriteLine($"Found more than one code file in {skillDirectory.FullName}");
                return 1;
            }
            
            var code = await File.ReadAllTextAsync(codeFiles[0].FullName);
            
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