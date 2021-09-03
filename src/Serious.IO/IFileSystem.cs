namespace Serious.IO
{
    /// <summary>
    /// Used to represent top-level file system operations.
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>Creates all directories and subdirectories in the specified path unless they already exist.</summary>
        /// <param name="path">The directory to create.</param>
        /// <returns>An object that represents the directory at the specified path. This object is returned
        /// regardless of whether a directory at the specified path already exists.</returns>
        IDirectoryInfo CreateDirectory(string path);

        /// <summary>
        /// Returns a directory at the specified path.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
        /// <returns>An object that represents the directory at the specified path. This object is returned
        /// regardless of whether a directory at the specified path already exists.</returns>
        IDirectoryInfo GetDirectory(string path);

        /// <summary>
        /// Returns a file at the specified path.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>An object that represents the file at the specified path. This object is returned
        /// regardless of whether a file at the specified path already exists.</returns>
        IFileInfo GetFile(string path);
    }
}