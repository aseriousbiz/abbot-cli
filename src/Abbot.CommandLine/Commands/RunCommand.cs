using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Serious.Abbot.CommandLine.Services;
using Serious.Abbot.Messages;

namespace Serious.Abbot.CommandLine.Commands
{
    public class RunCommand : Command
    {
        readonly IWorkspaceFactory _workspaceFactory;

        public RunCommand(IWorkspaceFactory workspaceFactory)
            : base("run", "Runs your skill code in the skill runner")
        {
            _workspaceFactory = workspaceFactory;
            Add(new Argument<string>("skill", () => string.Empty, "The name of the skill to run"));
            Add(new Argument<string>("arguments", () => ".", "The arguments to pass to the skill"));
            this.AddDirectoryOption();

            Handler = CommandHandler.Create<string, string, string?>(HandleRunCommandAsync);
        }

        internal async Task<int> HandleRunCommandAsync(string skill, string arguments, string? directory)
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

            var runRequest = new SkillRunRequest
            {
                Code = code,
                Arguments = arguments,
                Name = skill
            };

            var response = await AbbotApi.CreateInstance(workspace)
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