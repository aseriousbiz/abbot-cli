using System.IO;

namespace Serious.Abbot.CommandLine.IO
{
    public abstract class FileSystemInfoWrapper<T> : IFileSystemInfo
        where T : FileSystemInfo
    {
        protected FileSystemInfoWrapper(T fileSystemInfo)
        {
            InnerFileSystemInfo = fileSystemInfo;
        }
        
        protected T InnerFileSystemInfo { get; }

        public void Hide()
        {
            InnerFileSystemInfo.Attributes |= FileAttributes.Hidden;
        }

        public bool Exists => InnerFileSystemInfo.Exists;

        public string FullName => InnerFileSystemInfo.FullName;

        public override string ToString() => FullName;
    }
}