using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Threading.Tasks;
using Serious.Abbot.Messages;

namespace Serious.IO.CommandLine.Commands
{
    public class DeployCommand : AbbotCommand
    {
        public DeployCommand(ICommandContext commandContext)
            : base(commandContext, "deploy", $"Deploys local changes to the specified skill to {Program.Website}")
        {
            Add(new Argument<string>("skill", () => string.Empty, "The name of the skill"));
            this.AddDirectoryOption();
            Handler = CommandHandler.Create<string, string?>(HandleUploadCommandAsync);
        }

        async Task<int> HandleUploadCommandAsync(string skill, string? directory)
        {
            var workspace = GetWorkspace(directory);
            var skillWorkspace = workspace.GetSkillWorkspace(skill);

            if (!workspace.IsInitialized)
            {
                return HandleUninitializedWorkspace(workspace);
            }

            var codeResult = await skillWorkspace.GetCodeAsync();
            if (!codeResult.IsSuccess)
            {
                Console.Error.WriteLine(codeResult.ErrorMessage ?? "Unknown error occurred");
                return 1;
            }

            var code = codeResult.Code!;

            var previousCodeHash = await skillWorkspace.ReadConcurrencyFileAsync();

            var updateRequest = new SkillUpdateRequest
            {
                Code = code,
                PreviousCodeHash = previousCodeHash ?? string.Empty
            };

            var response = await CreateApiClient(workspace)
                .DeploySkillAsync(skill, updateRequest);

            if (!response.IsSuccessStatusCode)
            {
                return await response.HandleUnsuccessfulResponseAsync();
            }

            var result = response.Content;

            if (result is null)
            {
                Console.Out.WriteLine($"The skill directory has been corrupted. Please run `abbot get {skill} -d {directory}` to restore the state.");
                return 1;
            }

            await skillWorkspace.WriteConcurrencyFileAsync(result.NewCodeHash);
            Console.Out.WriteLine($"Skill {skill} updated.");
            return 0;
        }

    }
}
