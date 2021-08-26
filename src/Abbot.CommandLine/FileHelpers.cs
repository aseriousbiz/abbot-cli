using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Serious.Abbot.CommandLine
{
    /// <summary>
    /// Utilities for working with files.
    /// </summary>
    public static class FileHelpers
    {
        /// <summary>
        /// Creates a new file, write the contents to the file as a UTF8 encoded string, and then closes the file.
        /// If the target file already exists, it is overwritten. This version can handle writing to hidden files,
        /// unlike File.WriteAllText.
        /// </summary>
        /// <param name="path">The file to write to.</param>
        /// <param name="contents">The string to write to the file.</param>
        public static Task<FileInfo> WriteAllTextAsync(string path, string contents) => WriteAllTextAsync(path, contents, Encoding.UTF8);
     
        /// <summary>
        /// Creates a new file, write the contents to the file, and then closes the file. If the target file already
        /// exists, it is overwritten. This version can handle writing to hidden files, unlike File.WriteAllText.
        /// </summary>
        /// <param name="path">The file to write to.</param>
        /// <param name="contents">The string to write to the file.</param>
        /// <param name="encoding">The encoding to apply to the string.</param>
        static async Task<FileInfo> WriteAllTextAsync(string path, string contents, Encoding encoding)
        {
            // can't use WriteAllText because on windows File.Exists returns false if the file is hidden,
            // and then .net decides to create a new file and it all ends in tears
            await using (var fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                await using (var tw = new StreamWriter(fs, encoding, 1024, true))
                {
                    await tw.WriteAsync(contents);
                }
                fs.SetLength(fs.Position);
            }
            return new FileInfo(path);
        }

        /// <summary>
        /// Makes the specified file a hidden file.
        /// </summary>
        /// <param name="file">The file to make hidden</param>
        public static FileInfo HideFile(this FileInfo file)
        {
            file.Attributes |= FileAttributes.Hidden;
            return file;
        }
    }
}