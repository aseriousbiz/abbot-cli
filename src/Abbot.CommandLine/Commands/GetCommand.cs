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
        readonly IWorkspaceFactory _workspaceFactory;
        public const string SkillMetaFolder = ".meta";
        
        public GetCommand(IWorkspaceFactory workspaceFactory)
            : base("get", "Downloads the specified skill code into a directory named after the skill.")
        {
            _workspaceFactory = workspaceFactory;
            Add(new Argument<string>("skill", () => string.Empty, "The name of the skill"));
            this.AddDirectoryOption();
            this.AddOption<bool>(
                "--force",
                "-f",
                "If true, overwrites the local skill code if it exists even if it has changes.");
            Handler = CommandHandler.Create<string, string?, bool>(HandleGetCommandAsync);
        }

        async Task<int> HandleGetCommandAsync(string skill, string? directory, bool force)
        {
            var workspace = _workspaceFactory.GetWorkspace(directory);
            if (!workspace.IsInitialized)
            {
                var directoryType = workspace.DirectorySpecified ? "specified" : "current";
                Console.WriteLine($"The {directoryType} directory is not an Abbot Workspace. Either specify the path to an Abbot Workspace, or initialize a new one using `abbot init`");
                return 1;
            }
            
            var skillInfo = await GetSkillInfoAsync(skill, workspace);
            if (skillInfo is null)
            {
                return 1;
            }

            var skillWorkspace = await CreateSkillWorkspaceAsync(skillInfo, workspace, force);
            if (skillWorkspace is null)
            {
                return 1;
            }
            bool directoryExists = skillWorkspace.Exists;

            var verb = directoryExists ? "Updated" : "Created";
            
            Console.WriteLine(@$"{verb} skill directory {skillWorkspace.WorkingDirectory}
Edit the code in the directory. When you are ready to deploy it, run 

    abbot deploy {skill}
");
            return 0;
        }

        internal static async Task<SkillWorkspace?> CreateSkillWorkspaceAsync(SkillGetResponse skillInfo, Workspace workspace, bool force)
        {
            var skillWorkspace = workspace.GetSkillWorkspace(skillInfo.Name);

            bool directoryExists = skillWorkspace.Exists;
            
            if (!force && skillWorkspace.Exists && await skillWorkspace.HasLocalChangesAsync(skillInfo.Language))
            {
                Console.Write("You have local changes to the code that would be overwritten by getting the latest code.\nOverwrite local changes? Hit Y to overwrite, any other key to cancel: ");
                var key = Console.ReadKey();
                if (!(key.KeyChar is 'Y' or 'y'))
                {
                    Console.WriteLine("\nNo local changes were made");
                    return null;
                }

                Console.WriteLine("\nOverwriting local changes");
            }

            await skillWorkspace.CreateAsync(skillInfo);
            return skillWorkspace;
        }

        static async Task<SkillGetResponse?> GetSkillInfoAsync(string skill, Workspace workspace)
        {
            var response = await AbbotApi.CreateInstance(workspace).GetSkillAsync(skill);
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