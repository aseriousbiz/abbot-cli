namespace Serious.Abbot.CommandLine.IO
{
    public interface IDirectoryInfo : IFileSystemInfo
    {
        /// <summary>
        /// Retrieves a subdirectory by name.
        /// </summary>
        /// <param name="name">The name of the subfolder</param>
        /// <returns></returns>
        IDirectoryInfo GetSubdirectory(string name);

        /// <summary>
        /// Retrieves a file in the directory by name.
        /// </summary>
        /// <param name="name">The name of the file.</param>
        IFileInfo GetFile(string name);

        /// <summary>
        /// Creates the directory and all subdirectories. If it already exists, does nothing.
        /// </summary>
        void Create();
    }
}