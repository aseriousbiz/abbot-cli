using Serious.Abbot.CommandLine.IO;

namespace UnitTests.Fakes
{
    public abstract class FakeFileSystemInfo : IFileSystemInfo
    {
        protected FakeFileSystemInfo(string path)
        {
            FullName = path;
        }

        public bool Hidden { get; private set; }

        public void Hide()
        {
            Hidden = true;
        }

        public bool Exists { get; set; }
        public string FullName { get; }
        
        public override string ToString() => FullName;
    }
}