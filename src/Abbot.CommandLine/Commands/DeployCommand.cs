using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Refit;
using Serious.Abbot.CommandLine.Services;
using Serious.Abbot.Messages;

namespace Serious.Abbot.CommandLine.Commands
{
    public class DeployCommand : Command
    {
        public DeployCommand() : base("deploy", $"Deploys local changes to the specified skill to {Program.Website}")
        {
            Add(new Argument<string>("skill", () => string.Empty, "The name of the skill"));
            var directoryOption = new Option<string>("--directory", "The Abbot Skills folder. If omitted, assumes the current directory.");
            directoryOption.AddAlias("-d");
            AddOption(directoryOption);
            Handler = CommandHandler.Create<string, string>(HandleUploadCommandAsync);
        }
        
        static async Task<int> HandleUploadCommandAsync(string skill, string directory)
        {
            var environment = DevelopmentEnvironment.GetEnvironment(directory);
            if (!environment.IsInitialized)
            {
                var directoryType = directory == "." ? "current" : "specified";
                Console.WriteLine($"The {directoryType} directory is not an Abbot Skills folder. Either specify the directory where you've initialized an environment, or initialize a new one using `abbot init`");
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
            var concurrencyFilePath = Path.Combine(skillDirectory.FullName, ".concurrency");
            var previousCodeHash = await File.ReadAllTextAsync(concurrencyFilePath);

            var updateRequest = new SkillUpdateRequest
            {
                Code = code,
                PreviousCodeHash = previousCodeHash
            };

            var response = await AbbotApi.CreateInstance(environment)
                .DeploySkillAsync(skill, updateRequest);

            if (!response.IsSuccessStatusCode)
            {
                return await response.HandleUnsuccessfulResponseAsync();
            }

            var result = response.Content;
            
            if (result is null)
            {
                Console.WriteLine($"The skill directory has been corrupted. Please run `abbot download {skill} {directory}` to restore the state.");
                return 1;
            }

            await File.WriteAllTextAsync(concurrencyFilePath, result.NewCodeHash);
            Console.WriteLine($"Skill {skill} updated.");
            return 0;
        }

    }
}