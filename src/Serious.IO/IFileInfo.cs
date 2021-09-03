using System.IO;
using System.Threading.Tasks;

namespace Serious.IO
{
    public interface IFileInfo : IFileSystemInfo
    {
        /// <summary>
        /// Reads all text from the file at the specified path.
        /// </summary>
        /// <remarks>Returns null if the file doesn't exist. Otherwise returns the text of the file.</remarks>
        Task<string?> ReadAllTextAsync();
        
        /// <summary>
        /// Creates a new file, write the contents to the file as a UTF8 encoded string, and then closes the file.
        /// If the target file already exists, it is overwritten. This version can handle writing to hidden files,
        /// unlike File.WriteAllText.
        /// </summary>
        /// <param name="contents">The string to write to the file.</param>
        Task WriteAllTextAsync(string contents);

        /// <summary>Creates a <see cref="System.IO.StreamReader" /> with UTF8 encoding that reads from an existing text file.</summary>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="System.IO.FileNotFoundException">The file is not found.</exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// <see cref="System.IO.FileInfo.Name" /> is read-only or is a directory.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
        /// <returns>A new <see langword="StreamReader" /> with UTF8 encoding.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IO.FileInfo.OpenText?view=netcore-5.0">`FileInfo.OpenText` on docs.microsoft.com</a></footer>
        public StreamReader OpenText();

        /// <summary>
        /// Opens a <see cref="StreamWriter" /> to write to the file. If the file doesn't exist, creates the file.
        /// </summary>
        /// <returns>A new <see langword="StreamWriter" /> with UTF8 encoding.</returns>
        public StreamWriter OpenWriter();
    }
}