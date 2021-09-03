using System.IO;
using System.Text;
using System.Threading.Tasks;
using Serious.IO;
using Xunit;

namespace UnitTests.Fakes
{
    public class FakeFileInfo : FakeFileSystemInfo, IFileInfo
    {
        MemoryStream _contents = new();

        public FakeFileInfo(string path) : base(path)
        {
        }
        
        public Task<string?> ReadAllTextAsync()
        {
            var bytes = _contents.ToArray();
            var text = bytes is { Length: 0 } ? null : Encoding.UTF8.GetString(bytes);
            return Task.FromResult(text);
        }

        public Task WriteAllTextAsync(string contents)
        {
            Exists = true;
            _contents = new MemoryStream(Encoding.UTF8.GetBytes(contents));
            return Task.CompletedTask;
        }

        public StreamReader OpenText()
        {
            var readerStream = new MemoryStream(_contents.ToArray());           
            return new StreamReader(readerStream);
        }

        public StreamWriter OpenWriter()
        {
            Exists = true;
            _contents = new MemoryStream();
            return new StreamWriter(_contents);
        }
    }

    public class FakeFileInfoTests
    {
        [Fact]
        public async Task CanWriteAndReadToFile()
        {
            var file = new FakeFileInfo("./some-file.txt");
            await file.WriteAllTextAsync("This is the content");
            using var reader = file.OpenText();
            var content = await reader.ReadToEndAsync();
            Assert.Equal("This is the content", content);
        }
        
        [Fact]
        public async Task CanWriteUsingOpenWriterAndReadToFile()
        {
            var file = new FakeFileInfo("./some-file.txt");
            await using (var writer = file.OpenWriter())
            {
                await writer.WriteAsync("This is the content");
            }
            using var reader = file.OpenText();
            var content = await reader.ReadToEndAsync();
            Assert.Equal("This is the content", content);
        }
    }
}