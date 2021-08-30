namespace Serious.Abbot.CommandLine.Services
{
    /// <summary>
    /// Factory for creating Abbot <see cref="Workspace" /> instances.
    /// </summary>
    public interface IWorkspaceFactory
    {
        /// <summary>
        /// Creates an instance of a <see cref="Workspace"/> pointing to the specified directory.
        /// Note that this doesn't create the actual workspace on disk. <see cref="Workspace.EnsureAsync"/> has to be
        /// called on the <see cref="Workspace"/> instance to create the workspace on disk.
        /// </summary>
        /// <param name="directory">Path to the Abbot workspace directory.</param>
        Workspace GetWorkspace(string? directory);
    }
}