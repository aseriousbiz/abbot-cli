using Serious.IO.CommandLine.Services;

namespace Serious.IO.CommandLine
{
    /// <summary>
    /// Context useful for each invocation.
    /// </summary>
    public class CommandContext : ICommandContext
    {
        readonly IWorkspaceFactory _workspaceFactory;
        readonly IApiClientFactory _apiClientFactory;

        public CommandContext(
            IExtendedConsole console,
            IApiClientFactory apiClientFactory,
            IWorkspaceFactory workspaceFactory)
        {
            _workspaceFactory = workspaceFactory;
            _apiClientFactory = apiClientFactory;
            Console = console;
        }

        /// <summary>
        /// The Console to write to.
        /// </summary>
        public IExtendedConsole Console { get; }
        
        /// <summary>
        /// Creates an Abbot API client using the token stored in the <see cref="Workspace" />.
        /// </summary>
        /// <param name="workspace">The workspace to use for the API client.</param>
        /// <returns>An <see cref="IAbbotApi"/> instance.</returns>
        public IAbbotApi CreateApiClient(Workspace workspace) => _apiClientFactory.Create(workspace);

        /// <summary>
        /// Creates an instance of a <see cref="Workspace"/> pointing to the specified directory.
        /// Note that this doesn't create the actual workspace on disk. <see cref="Workspace.EnsureAsync"/> has to be
        /// called on the <see cref="Workspace"/> instance to create the workspace on disk.
        /// </summary>
        /// <param name="directory">Path to the Abbot workspace directory.</param>
        public Workspace GetWorkspace(string? directory) => _workspaceFactory.GetWorkspace(directory);
    }
}