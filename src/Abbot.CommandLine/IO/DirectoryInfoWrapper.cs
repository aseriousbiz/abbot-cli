using System.IO;

namespace Serious.Abbot.CommandLine.IO
{
    public class DirectoryInfoWrapper : FileSystemInfoWrapper<DirectoryInfo>, IDirectoryInfo
    {
        public DirectoryInfoWrapper(string path)
            : this(new DirectoryInfo(path is {Length: 0} ? "." : path))
        {
        }

        DirectoryInfoWrapper(DirectoryInfo directoryInfo) : base(directoryInfo)
        {
        }

        public IDirectoryInfo GetSubdirectory(string name) => new DirectoryInfoWrapper(Path.Combine(FullName, name));

        public IFileInfo GetFile(string name) => new FileInfoWrapper(Path.Combine(FullName, name));

        public void Create() => InnerFileSystemInfo.Create();
    }
}