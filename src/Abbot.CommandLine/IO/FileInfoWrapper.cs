using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Serious.Abbot.CommandLine.IO
{
    public class FileInfoWrapper : IFileInfo
    {
        readonly FileInfo _fileInfo;

        public FileInfoWrapper(string path) : this(new FileInfo(path))
        {
        }

        public FileInfoWrapper(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
        }

        public void Hide() => _fileInfo.Attributes |= FileAttributes.Hidden;

        public bool Exists => _fileInfo.Exists;
        
        public string FullName => _fileInfo.FullName;

        public FileStream Create() => _fileInfo.Create();

        public Task<byte[]> ReadAllBytesAsync() => File.ReadAllBytesAsync(FullName);
        
        public Task<string> ReadAllTextAsync() => File.ReadAllTextAsync(FullName);
        public async Task WriteAllTextAsync(string contents)
        {
            // can't use WriteAllText because on windows File.Exists returns false if the file is hidden,
            // and then .net decides to create a new file and it all ends in tears
            await using (var fs = new FileStream(FullName, FileMode.OpenOrCreate))
            {
                await using (var tw = new StreamWriter(fs, Encoding.UTF8, 1024, true))
                {
                    await tw.WriteAsync(contents);
                }
                fs.SetLength(fs.Position);
            }
        }

        public override string ToString() => _fileInfo.FullName;
    }
}