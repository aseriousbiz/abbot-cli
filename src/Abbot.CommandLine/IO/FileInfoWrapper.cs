using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Serious.Abbot.CommandLine.IO
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
        
        public Task<string> ReadAllTextAsync() => File.ReadAllTextAsync(FullName);
        
        public async Task WriteAllTextAsync(string contents)
        {
            // can't use WriteAllText because on windows File.Exists returns false if the file is hidden,
            // and then .net decides to create a new file and it all ends in tears
            await using var fs = new FileStream(FullName, FileMode.OpenOrCreate);
            await using var tw = new StreamWriter(fs, Encoding.UTF8, 1024, true);
            await tw.WriteAsync(contents);
            fs.SetLength(fs.Position);
        }

        public async ValueTask WriteAllBytesAsync(byte[] bytes)
        {
            await using var fs = new FileStream(FullName, FileMode.OpenOrCreate);
            await fs.WriteAsync(bytes);
        }
    }
}