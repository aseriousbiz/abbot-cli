using System;
using Serious.Abbot.CommandLine.IO;

namespace Serious.Abbot.CommandLine.Services
{
    /// <summary>
    /// Factory for creating Abbot <see cref="Workspace" /> instances backed by the file system.
    /// </summary>
    public class WorkspaceFactory : IWorkspaceFactory
    {
        readonly Func<string, IDirectoryInfo> _directoryFactory;

        /// <summary>
        /// Constructs an instance of <see cref="WorkspaceFactory" />.
        /// </summary>
        public WorkspaceFactory() : this(directory => new DirectoryInfoWrapper(directory))
        {
        }

        /// <summary>
        /// Constructs an instance of <see cref="WorkspaceFactory" /> using the supplied function to retrieve an
        /// <see cref="IDirectoryInfo" /> from a directory path. This is useful for unit tests.
        /// </summary>
        /// <param name="directoryFactory">A factory to turn paths into <see cref="IDirectoryInfo"/> instances.</param>
        public WorkspaceFactory(Func<string, IDirectoryInfo> directoryFactory)
        {
            _directoryFactory = directoryFactory;
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
            var workingDirectory = _directoryFactory(directory ?? ".");
            return new Workspace(workingDirectory, directorySpecified);
        }
    }
}