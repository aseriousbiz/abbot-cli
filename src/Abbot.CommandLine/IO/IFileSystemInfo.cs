namespace Serious.Abbot.CommandLine.IO
{
    public interface IFileSystemInfo
    {
        /// <summary>
        /// Makes the file system item as hidden.
        /// </summary>
        void Hide();
        
        /// <summary>
        /// Whether or not the file system item exists.
        /// </summary>
        bool Exists { get; }
        
        /// <summary>
        /// The full path to the file system item.
        /// </summary>
        string FullName { get; }
    }
}