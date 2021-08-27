using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Serious.Abbot.CommandLine.Services;
using Serious.Abbot.Messages;

namespace Serious.Abbot.CommandLine.Commands
{
    public class GetCommand : Command
    {
        readonly IDevelopmentEnvironmentFactory _developmentEnvironmentFactory;
        public const string SkillMetaFolder = ".meta";
        
        public GetCommand(IDevelopmentEnvironmentFactory developmentEnvironmentFactory)
            : base("get", "Downloads the specified skill code into a directory named after the skill.")
        {
            _developmentEnvironmentFactory = developmentEnvironmentFactory;
            Add(new Argument<string>("skill", () => string.Empty, "The name of the skill"));
            var directoryOption = new Option<string?>("--directory", "The Abbot Skills folder. If omitted, assumes the current directory.");
            directoryOption.AddAlias("-d");
            AddOption(directoryOption);
            var forceOption = new Option<bool>("--force", "If true, overwrites the local skill code if it exists even if it has changes.");
            forceOption.AddAlias("-f");
            AddOption(forceOption);
            Handler = CommandHandler.Create<string, string?, bool>(HandleDownloadCommandAsync);
        }

        async Task<int> HandleDownloadCommandAsync(string skill, string? directory, bool force)
        {
            var environment = _developmentEnvironmentFactory.GetDevelopmentEnvironment(directory);
            if (!environment.IsInitialized)
            {
                var directoryType = environment.DirectorySpecified ? "specified" : "current";
                Console.WriteLine($"The {directoryType} directory is not an Abbot Skills folder. Either specify the directory where you've initialized an environment, or initialize a new one using `abbot init`");
                return 1;
            }
            
            var skillInfo = await GetSkillInfoAsync(skill, environment);
            if (skillInfo is null)
            {
                return 1;
            }

            var skillEnvironment = environment.GetSkillEnvironment(skill);

            bool directoryExists = skillEnvironment.Exists;
            
            if (!force && skillEnvironment.Exists && await skillEnvironment.HasLocalChangesAsync(skillInfo.Language))
            {
                Console.Write("You have local changes to the code that would be overwritten by getting the latest code.\nOverwrite local changes? Hit Y to overwrite, any other key to cancel: ");
                var key = Console.ReadKey();
                if (!(key.KeyChar is 'Y' or 'y'))
                {
                    Console.WriteLine("\nNo local changes were made");
                    return 1;
                }

                Console.WriteLine("\nOverwriting local changes");
            }

            await skillEnvironment.CreateAsync(skillInfo);

            var verb = directoryExists ? "Updated" : "Created";
            
            Console.WriteLine(@$"{verb} skill directory {skillEnvironment.WorkingDirectory}
Edit the code in the directory. When you are ready to deploy it, run 

    abbot deploy {skill}
");
            return 0;
        }

        static async Task<SkillGetResponse?> GetSkillInfoAsync(string skill, DevelopmentEnvironment environment)
        {
            var response = await AbbotApi.CreateInstance(environment).GetSkillAsync(skill);
            if (!response.IsSuccessStatusCode)
            {
                await response.HandleUnsuccessfulResponseAsync();
                return null;
            }

            if (response.Content is null)
            {
                await Console.Error.WriteLineAsync("Response content is null");
                return null;
            }

            return response.Content;
        }
    }
}