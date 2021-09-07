using System.Collections.Generic;
using Serious.IO;
using UnitTests;
using UnitTests.Fakes;

namespace TestFakes
{
    public class FakeFileSystem : IFileSystem
    {
        readonly Dictionary<string, IDirectoryInfo> _directories = new();
        readonly Dictionary<string, IFileInfo> _files = new();

        public IDirectoryInfo CreateDirectory(string path)
        {
            return _directories.GetOrCreate(path, dir => new FakeDirectoryInfo(dir));
        }

        public IDirectoryInfo GetDirectory(string path)
        {
            return _directories.GetOrCreate(path, dir => new FakeDirectoryInfo(dir));
        }

        public IFileInfo GetFile(string path) => _files.GetOrCreate(path, dir => new FakeFileInfo(dir));
    }
}