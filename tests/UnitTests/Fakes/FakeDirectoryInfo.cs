using System;
using System.Collections.Generic;
using System.IO;
using Serious.Abbot.CommandLine.IO;

namespace UnitTests.Fakes
{
    public class FakeDirectoryInfo : FakeFileSystemInfo, IDirectoryInfo
    {
        readonly Dictionary<string, IFileInfo> _files = new();
        readonly Dictionary<string, IDirectoryInfo> _subdirectories = new();

        public FakeDirectoryInfo(string path) : base(path)
        {
        }

        public IDirectoryInfo GetSubdirectory(string name)
        {
            var path = Path.Combine(FullName, name);

            if (_subdirectories.TryGetValue(path, out var directory))
            {
                return directory;
            }

            var trackedDirectory = new FakeDirectoryInfo(path);
            _subdirectories.Add(path, trackedDirectory);
            return trackedDirectory;
        }

        public IFileInfo GetFile(string name)
        {
            var filePath = Path.Combine(FullName, name);

            if (_files.TryGetValue(filePath, out var file))
            {
                return file;
            }

            var trackedFile = new FakeFileInfo(filePath);
            _files.Add(filePath, trackedFile);
            return trackedFile;
        }

        public void Create()
        {
            Exists = true;
        }

        /// <summary>
        /// Adds a file to this fake directory.
        /// </summary>
        /// <param name="file">The file to add.</param>
        public void AddFile(IFileInfo file)
        {
            ValidateFileSystemInfo(file);

            _files.Add(file.FullName, file);
        }

        /// <summary>
        /// Adds a directory to this fake directory.
        /// </summary>
        /// <param name="directory"></param>
        public void AddSubDirectory(IDirectoryInfo directory)
        {
            ValidateFileSystemInfo(directory);

            _subdirectories.Add(directory.FullName, directory);
        }

        void ValidateFileSystemInfo(IFileSystemInfo fileSystemInfo)
        {
            if (fileSystemInfo.FullName is not { Length: > 0 })
            {
                throw new ArgumentException("The file/directory needs to have a FullName");
            }
            
            if (FullName is not { Length: > 0 })
            {
                throw new InvalidOperationException("Cannot add a file/directory to a directory without a FullName");
            }

            var fileDirectory = Path.GetDirectoryName(fileSystemInfo.FullName);
            if (fileDirectory is null)
            {
                throw new ArgumentException("File or directory doesn't have a FullName.");
            }

            if (!fileDirectory.Equals(FullName, StringComparison.Ordinal))
            {
                throw new ArgumentException("File/Directory is not a subdirectory of this directory.");
            }

            if (!fileSystemInfo.Exists)
            {
                throw new ArgumentException("Only add files/directories that exist");
            }
        }
    }
}