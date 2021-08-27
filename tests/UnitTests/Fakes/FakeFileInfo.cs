using System;
using System.Threading.Tasks;
using Serious.Abbot.CommandLine.IO;

namespace UnitTests.Fakes
{
    public class FakeFileInfo : FakeFileSystemInfo, IFileInfo
    {
        byte[]? _stored;
        string? _storedText;
        
        public FakeFileInfo(string path) : base(path)
        {
        }

        public Task<byte[]> ReadAllBytesAsync()
        {
            return Task.FromResult(_stored ?? Array.Empty<byte>());
        }

        public Task<string> ReadAllTextAsync()
        {
            return Task.FromResult(_storedText ?? String.Empty);
        }

        public Task WriteAllTextAsync(string contents)
        {
            Exists = true;
            _storedText = contents;
            return Task.CompletedTask;
        }

        public ValueTask WriteAllBytesAsync(byte[] bytes)
        {
            Exists = true;
            _stored = bytes;
            return ValueTask.CompletedTask;
        }
    }
}