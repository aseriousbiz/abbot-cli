using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Threading.Tasks;
using Serious.IO.CommandLine.Services;
using Serious.IO.Messages;

namespace Serious.IO.CommandLine.Commands
{
    public class GetCommand : AbbotCommand
    {
        public const string SkillMetaFolder = ".meta";
        
        public GetCommand(ICommandContext commandContext)
            : base(commandContext, "get", "Downloads the specified skill code into a directory named after the skill.")
        {
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
            var workspace = GetWorkspace(directory);
            if (!workspace.IsInitialized)
            {
                var directoryType = workspace.DirectorySpecified ? "specified" : "current";
                Console.Out.WriteLine($"The {directoryType} directory is not an Abbot Workspace. Either specify the path to an Abbot Workspace, or initialize a new one using `abbot init`");
                return 1;
            }
            
            var skillInfo = await GetSkillInfoAsync(skill, workspace);
            if (skillInfo is null)
            {
                return 1;
            }

            var skillWorkspace = await CreateSkillWorkspaceAsync(skillInfo, workspace, Console, force);
            if (skillWorkspace is null)
            {
                return 1;
            }
            bool directoryExists = skillWorkspace.Exists;

            var verb = directoryExists ? "Updated" : "Created";
            
            Console.Out.WriteLine(@$"{verb} skill directory {skillWorkspace.WorkingDirectory}
Edit the code in the directory. When you are ready to deploy it, run 

    abbot deploy {skill}
");
            return 0;
        }

        internal static async Task<SkillWorkspace?> CreateSkillWorkspaceAsync(
            SkillGetResponse skillInfo,
            Workspace workspace,
            IExtendedConsole console,
            bool force)
        {
            var skillWorkspace = workspace.GetSkillWorkspace(skillInfo.Name);
            
            if (!force && skillWorkspace.Exists && await skillWorkspace.HasLocalChangesAsync(skillInfo.Language))
            {
                console.Out.Write("You have local changes to the code that would be overwritten by getting the latest code.\nOverwrite local changes? Hit Y to overwrite, any other key to cancel: ");
                var key = console.ReadKey();
                if (!(key.KeyChar is 'Y' or 'y'))
                {
                    console.Out.WriteLine("\nNo local changes were made");
                    return null;
                }

                console.Out.WriteLine("\nOverwriting local changes");
            }

            await skillWorkspace.CreateAsync(skillInfo);
            return skillWorkspace;
        }

        async Task<SkillGetResponse?> GetSkillInfoAsync(string skill, Workspace workspace)
        {
            var response = await CreateApiClient(workspace).GetSkillAsync(skill);
            if (!response.IsSuccessStatusCode)
            {
                await response.HandleUnsuccessfulResponseAsync();
                return null;
            }

            if (response.Content is null)
            {
                Console.Error.WriteLine("Response content is null");
                return null;
            }

            return response.Content;
        }
    }
}