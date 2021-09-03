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
        readonly Dictionary<string, string> _environmentVariables = new();

        public IDirectoryInfo CreateDirectory(string path)
        {
            return _directories.GetOrCreate(path, dir => new FakeDirectoryInfo(dir));
        }

        public IDirectoryInfo GetDirectory(string path)
        {
            return _directories.GetOrCreate(path, dir => new FakeDirectoryInfo(dir));
        }

        public IFileInfo GetFile(string path) => _files.GetOrCreate(path, dir => new FakeFileInfo(dir));

        public void SetEnvironmentVariable(string variable, string value) => _environmentVariables[variable] = value;

        public string? GetEnvironmentVariable(string variable) =>
            _environmentVariables.TryGetValue(variable, out var value)
                ? value
                : null;
    }
}