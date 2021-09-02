using System.CommandLine;
using System.CommandLine.IO;
using Serious.Abbot.CommandLine.IO;
using Serious.Abbot.CommandLine.Services;

namespace Serious.Abbot.CommandLine.Commands
{
    public abstract class AbbotCommand : Command
    {
        readonly ICommandContext _commandContext;

        protected AbbotCommand(ICommandContext commandContext, string name, string? description = null)
            : base(name, description)
        {
            _commandContext = commandContext;
            Console = commandContext.Console;
        }

        /// <summary>
        /// Creates an instance of a <see cref="Workspace"/> pointing to the specified directory.
        /// Note that this doesn't create the actual workspace on disk. <see cref="Workspace.EnsureAsync"/> has to be
        /// called on the <see cref="Workspace"/> instance to create the workspace on disk.
        /// </summary>
        /// <param name="directory">Path to the Abbot workspace directory.</param>
        protected Workspace GetWorkspace(string? directory) => _commandContext.GetWorkspace(directory);

        /// <summary>
        /// Creates the Abbot API client using the API token stored in the workspace.
        /// </summary>
        /// <param name="workspace">The workspace</param>
        protected IAbbotApi CreateApiClient(Workspace workspace) => _commandContext.CreateApiClient(workspace);

        /// <summary>
        /// The Console to write to.
        /// </summary>
        protected IExtendedConsole Console { get; }

        protected int HandleUninitializedWorkspace(Workspace workspace)
        {
            var directoryType = workspace.DirectorySpecified ? "specified" : "current";
            Console.Error.WriteLine($"The {directoryType} directory is not an Abbot Workspace. Either specify the path to an Abbot Workspace, or initialize a new one using `abbot init`");
            return 1;
        }
    }
}