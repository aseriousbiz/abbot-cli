using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Threading.Tasks;
using Serious.Abbot.Messages;
using Serious.IO.CommandLine.Services;

namespace Serious.IO.CommandLine.Commands
{
    public class RunCommand : AbbotCommand
    {
        public RunCommand(ICommandContext commandContext)
            : base(commandContext, "run", "Runs your skill code in the skill runner")
        {
            Add(new Argument<string>("skill", () => string.Empty, "The name of the skill to run"));
            Add(new Argument<string>("arguments", () => ".", "The arguments to pass to the skill"));
            this.AddDirectoryOption();
            this.AddOption<bool>("--deployed", "-s", "If specified, runs the deployed version of the skill. Aka, the version on the server.");

            Handler = CommandHandler.Create<string, string, string?, bool>(HandleRunCommandAsync);
        }

        internal async Task<string?> GetCodeToRunAsync(string skill, Workspace workspace, bool deployed)
        {
            if (deployed)
            {
                return string.Empty;
            }

            var skillWorkspace = workspace.GetSkillWorkspace(skill);

            var codeResult = await skillWorkspace.GetCodeAsync();
            if (!codeResult.IsSuccess)
            {
                Console.Error.WriteLine(codeResult.ErrorMessage ?? "Unknown error occurred");
                return null;
            }

            return codeResult.Code!;
        }

        async Task<int> HandleRunCommandAsync(string skill, string arguments, string? directory, bool deployed)
        {
            var workspace = GetWorkspace(directory);

            if (!workspace.IsInitialized)
            {
                return HandleUninitializedWorkspace(workspace);
            }

            var code = await GetCodeToRunAsync(skill, workspace, deployed);
            if (code is null)
            {
                return 1;
            }

            return await RunCodeAsync(skill, arguments, code, workspace, deployed);
        }

        internal async Task<int> RunCodeAsync(string skill, string arguments, string code, Workspace workspace, bool deployed)
        {
            var runRequest = new SkillRunRequest
            {
                Code = code,
                Arguments = arguments,
                Name = skill
            };

            var client = CreateApiClient(workspace);

            var response = await (deployed
                ? client.RunDeployedSkillAsync(skill, runRequest)
                : client.RunSkillAsync(skill, runRequest));

            if (!response.IsSuccessStatusCode)
            {
                return await response.HandleUnsuccessfulResponseAsync();
            }

            if (response.Content?.Replies is null)
            {
                Console.Error.WriteLine("Response content is null");
                return 1;
            }

            Console.Out.WriteLine(string.Join("\n", response.Content.Replies));
            return 0;
        }
    }
}
