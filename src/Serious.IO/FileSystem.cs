using System.IO;

namespace Serious.IO
{
    /// <summary>
    /// Used to represent top-level file system operations.
    /// </summary>
    public class FileSystem : IFileSystem
    {
        /// <summary>Creates all directories and subdirectories in the specified path unless they already exist.</summary>
        /// <param name="path">The directory to create.</param>
        /// <returns>An object that represents the directory at the specified path. This object is returned
        /// regardless of whether a directory at the specified path already exists.</returns>
        public IDirectoryInfo CreateDirectory(string path)
        {
            var directory = Directory.CreateDirectory(path);
            return new DirectoryInfoWrapper(directory);
        }

        /// <summary>
        /// Returns a directory at the specified path.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
        /// <returns>An object that represents the directory at the specified path. This object is returned
        /// regardless of whether a directory at the specified path already exists.</returns>
        public IDirectoryInfo GetDirectory(string path)
        {
            return new DirectoryInfoWrapper(new DirectoryInfo(path));
        }

        /// <summary>
        /// Returns a file at the specified path.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>An object that represents the file at the specified path. This object is returned
        /// regardless of whether a file at the specified path already exists.</returns>
        public IFileInfo GetFile(string path) => new FileInfoWrapper(path);
    }
}