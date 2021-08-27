using System.IO;
using System.Threading.Tasks;

namespace Serious.Abbot.CommandLine.IO
{
    public interface IFileInfo : IFileSystemInfo
    {
        /// <summary>
        /// Creates the file and returns a stream to write to it.
        /// </summary>
        FileStream Create();

        /// <summary>
        /// Reads all the bytes of the file into a byte array.
        /// </summary>
        Task<byte[]> ReadAllBytesAsync();
        
        /// <summary>
        /// Reads all text from the file at the specified path.
        /// </summary>
        Task<string> ReadAllTextAsync();
        
        /// <summary>
        /// Creates a new file, write the contents to the file as a UTF8 encoded string, and then closes the file.
        /// If the target file already exists, it is overwritten. This version can handle writing to hidden files,
        /// unlike File.WriteAllText.
        /// </summary>
        /// <param name="contents">The string to write to the file.</param>
        Task WriteAllTextAsync(string contents);
    }
}