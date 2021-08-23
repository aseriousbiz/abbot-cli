using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Serious.Abbot.CommandLine.Services;
using Serious.Abbot.Messages;

namespace Serious.Abbot.CommandLine.Commands
{
    public class DeployCommand : Command
    {
        public DeployCommand() : base("deploy", $"Deploys local changes to the specified skill to {Program.Website}")
        {
            Add(new Argument<string>("skill", () => string.Empty, "The name of the skill"));
            Add(new Argument<string>("directory", () => ".", "The Abbot Skills Development Environment directory. If omitted, assumes the current directory."));
            Handler = CommandHandler.Create<string, string>(HandleUploadCommandAsync);
        }
        
        static async Task<int> HandleUploadCommandAsync(string skill, string directory)
        {
            var environment = DevelopmentEnvironment.GetEnvironment(directory);
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
                if (response.StatusCode is HttpStatusCode.Conflict)
                {
                    Console.WriteLine(@"Could not update the skill because it has been updated by someone else since you started working on it");

                    var conflict = await response.Content.ReadFromJsonAsync<SkillUpdateConflict>();
                    
                    if (conflict is not null)
                    {
                        Console.WriteLine($@"
    Last Modified:  {conflict.LastModified}
    Modified By:    {conflict.LastModifiedBy.Name} ({conflict.LastModifiedBy.PlatformUserId})
");
                    }
                    
                    Console.WriteLine("You can use `abbot get` to retrieve the latest changes and then apply your changes. We recommend using Git to manage your local changes so you can stash your changes first.");
                    return 1;
                }

                var message = response.StatusCode switch
                {
                    HttpStatusCode.Found => "Could not find a skill of that name.",
                    HttpStatusCode.InternalServerError => "An error occurred on the server. Contact support@aseriousbusiness.com to learn more. It's their fault.",
                    HttpStatusCode.Unauthorized => "The API Key you provided is not valid or expired. Run \"abbot auth\" to authenticate again.",
                    HttpStatusCode.Forbidden => "You do not have permission to edit that skill. Contact your administrators to request permission.",
                    _ => $"Received a {response.StatusCode} response from the server"
                };
                await Console.Error.WriteLineAsync(message);
                return 1;
            }

            var result = await response.Content.ReadFromJsonAsync<SkillUpdateResponse>();
            
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