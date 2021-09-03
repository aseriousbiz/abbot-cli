using System;

namespace Serious.IO.CommandLine.Services
{
    /// <summary>
    /// Factory for creating Abbot <see cref="Workspace" /> instances backed by the file system.
    /// </summary>
    public class WorkspaceFactory : IWorkspaceFactory
    {
        readonly Func<IFileInfo, ITokenStore> _tokenConstructor;
        readonly Func<IDirectoryInfo, bool, ITokenStore, Workspace> _workspaceConstructor;
        readonly IFileSystem _fileSystem;

        /// <summary>
        /// Constructs an instance of <see cref="WorkspaceFactory" /> using the supplied function to retrieve an
        /// <see cref="IDirectoryInfo" /> from a directory path. This is useful for unit tests.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="tokenStoreConstructor">Method for creating a <see cref="ITokenStore"/>.</param>
        /// <param name="workspaceConstructor">Method for creating a workspace.</param>
        public WorkspaceFactory(
            IFileSystem fileSystem,
            Func<IFileInfo, ITokenStore> tokenStoreConstructor,
            Func<IDirectoryInfo, bool, ITokenStore, Workspace> workspaceConstructor)
        {
            _fileSystem = fileSystem;
            _tokenConstructor = tokenStoreConstructor;
            _workspaceConstructor = workspaceConstructor;
        }
        
        /// <summary>
        /// Creates an instance of a <see cref="Workspace"/> pointing to the specified directory.
        /// Note that this doesn't create the actual workspace on disk. <see cref="Workspace.EnsureAsync"/> has to be
        /// called on the <see cref="Workspace"/> instance to create the workspace on disk.
        /// </summary>
        /// <param name="directory">Path to the Abbot workspace directory.</param>
        public Workspace GetWorkspace(string? directory)
        {
            var directorySpecified = directory is { Length: > 0 };
            var workingDirectory = _fileSystem.GetDirectory(directory is {Length: > 0} ? directory : ".");
            var metadataDirectory = workingDirectory.GetSubdirectory(".abbot");
            var secretIdFile = metadataDirectory.GetFile("SecretsId");
            var tokenStore = _tokenConstructor(secretIdFile);
            return _workspaceConstructor(workingDirectory, directorySpecified, tokenStore);
        }
    }
}