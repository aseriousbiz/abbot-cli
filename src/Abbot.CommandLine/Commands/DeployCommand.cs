using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Serious.Abbot.CommandLine.Services;
using Serious.Abbot.Messages;

namespace Serious.Abbot.CommandLine.Commands
{
    public class DeployCommand : Command
    {
        readonly IDevelopmentEnvironmentFactory _developmentEnvironmentFactory;

        public DeployCommand(IDevelopmentEnvironmentFactory developmentEnvironmentFactory)
            : base("deploy", $"Deploys local changes to the specified skill to {Program.Website}")
        {
            _developmentEnvironmentFactory = developmentEnvironmentFactory;
            Add(new Argument<string>("skill", () => string.Empty, "The name of the skill"));
            var directoryOption = new Option<string?>("--directory", "The Abbot Skills folder. If omitted, assumes the current directory.");
            directoryOption.AddAlias("-d");
            AddOption(directoryOption);
            Handler = CommandHandler.Create<string, string>(HandleUploadCommandAsync);
        }
        
        async Task<int> HandleUploadCommandAsync(string skill, string? directory)
        {
            var environment = _developmentEnvironmentFactory.GetDevelopmentEnvironment(directory ?? ".");
            var skillEnvironment = environment.GetSkillEnvironment(skill);

            var codeResult = await skillEnvironment.GetCodeAsync(environment);
            if (!codeResult.IsSuccess)
            {
                await Console.Error.WriteLineAsync(codeResult.ErrorMessage);
                return 1;
            }
            
            var code = codeResult.Code!;

            var previousCodeHash = await skillEnvironment.ReadConcurrencyFileAsync();

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

            await skillEnvironment.WriteConcurrencyFileAsync(result.NewCodeHash);
            Console.WriteLine($"Skill {skill} updated.");
            return 0;
        }

    }
}