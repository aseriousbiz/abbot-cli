using Serious.Abbot.CommandLine.IO;
using Serious.Abbot.CommandLine.Services;

namespace Serious.Abbot.CommandLine
{
    /// <summary>
    /// Context useful for each invocation.
    /// </summary>
    public interface ICommandContext
    {
        /// <summary>
        /// The Console to write to.
        /// </summary>
        IExtendedConsole Console { get; }

        /// <summary>
        /// Creates an Abbot API client using the token stored in the <see cref="Workspace" />.
        /// </summary>
        /// <param name="workspace">The workspace to use for the API client.</param>
        /// <returns>An <see cref="IAbbotApi"/> instance.</returns>
        IAbbotApi CreateApiClient(Workspace workspace);
        
        /// <summary>
        /// Creates an instance of a <see cref="Workspace"/> pointing to the specified directory.
        /// Note that this doesn't create the actual workspace on disk. <see cref="Workspace.EnsureAsync"/> has to be
        /// called on the <see cref="Workspace"/> instance to create the workspace on disk.
        /// </summary>
        /// <param name="directory">Path to the Abbot workspace directory.</param>
        Workspace GetWorkspace(string? directory);
    }
}