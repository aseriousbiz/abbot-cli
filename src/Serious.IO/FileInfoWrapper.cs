using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Serious.IO
{
    public class FileInfoWrapper : FileSystemInfoWrapper<FileInfo>, IFileInfo
    {
        public FileInfoWrapper(string path) : this(new FileInfo(path))
        {
        }

        FileInfoWrapper(FileInfo fileInfo) : base(fileInfo)
        {
        }
        
        public Task<byte[]> ReadAllBytesAsync() => File.ReadAllBytesAsync(FullName);
        
        public async Task<string?> ReadAllTextAsync()
        {
            if (!InnerFileSystemInfo.Exists)
            {
                return null;
            }

            using var reader = InnerFileSystemInfo.OpenText();
            return await reader.ReadToEndAsync();
        }

        public async Task WriteAllTextAsync(string contents)
        {
            // can't use WriteAllText because on windows File.Exists returns false if the file is hidden,
            // and then .net decides to create a new file and it all ends in tears
            await using var fs = new FileStream(FullName, FileMode.OpenOrCreate);
            await using var tw = new StreamWriter(fs, Encoding.UTF8, 1024, true);
            await tw.WriteAsync(contents);
            fs.SetLength(fs.Position);
        }

        public StreamReader OpenText() => InnerFileSystemInfo.OpenText();
        public StreamWriter OpenWriter() => new(InnerFileSystemInfo.OpenWrite());
    }
}