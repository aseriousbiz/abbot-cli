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
        readonly IWorkspaceFactory _workspaceFactory;

        public DeployCommand(IWorkspaceFactory workspaceFactory)
            : base("deploy", $"Deploys local changes to the specified skill to {Program.Website}")
        {
            _workspaceFactory = workspaceFactory;
            Add(new Argument<string>("skill", () => string.Empty, "The name of the skill"));
            this.AddDirectoryOption();
            Handler = CommandHandler.Create<string, string?>(HandleUploadCommandAsync);
        }
        
        async Task<int> HandleUploadCommandAsync(string skill, string? directory)
        {
            var workspace = _workspaceFactory.GetWorkspace(directory);
            var skillWorkspace = workspace.GetSkillWorkspace(skill);

            var codeResult = await skillWorkspace.GetCodeAsync(workspace);
            if (!codeResult.IsSuccess)
            {
                await Console.Error.WriteLineAsync(codeResult.ErrorMessage);
                return 1;
            }
            
            var code = codeResult.Code!;

            var previousCodeHash = await skillWorkspace.ReadConcurrencyFileAsync();

            var updateRequest = new SkillUpdateRequest
            {
                Code = code,
                PreviousCodeHash = previousCodeHash
            };

            var response = await AbbotApi.CreateInstance(workspace)
                .DeploySkillAsync(skill, updateRequest);

            if (!response.IsSuccessStatusCode)
            {
                return await response.HandleUnsuccessfulResponseAsync();
            }

            var result = response.Content;
            
            if (result is null)
            {
                Console.WriteLine($"The skill directory has been corrupted. Please run `abbot get {skill} -d {directory}` to restore the state.");
                return 1;
            }

            await skillWorkspace.WriteConcurrencyFileAsync(result.NewCodeHash);
            Console.WriteLine($"Skill {skill} updated.");
            return 0;
        }

    }
}