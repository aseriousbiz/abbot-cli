using System.IO;

namespace Serious.Abbot.CommandLine.IO
{
    public class DirectoryInfoWrapper : IDirectoryInfo
    {
        readonly DirectoryInfo _directoryInfo;

        public DirectoryInfoWrapper(string path)
            : this(new DirectoryInfo(path is {Length: 0} ? "." : path))
        {
        }

        public DirectoryInfoWrapper(DirectoryInfo directoryInfo)
        {
            _directoryInfo = directoryInfo;
        }

        public void Hide()
        {
            _directoryInfo.Attributes |= _directoryInfo.Attributes;
        }
        
        public bool Exists => _directoryInfo.Exists;

        public string FullName => _directoryInfo.FullName;
        
        public IDirectoryInfo GetSubdirectory(string name) => new DirectoryInfoWrapper(Path.Combine(_directoryInfo.FullName, name));

        public IFileInfo GetFile(string name) => new FileInfoWrapper(Path.Combine(_directoryInfo.FullName, name));

        public void Create() => _directoryInfo.Create();

        public override string ToString() => _directoryInfo.FullName;
    }
}