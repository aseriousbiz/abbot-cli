using Serious.IO;
using Serious.IO.CommandLine.Services;

namespace UnitTests.Fakes
{
    public class FakeWorkspace : Workspace
    {
        public FakeWorkspace(
            IDirectoryInfo workingDirectory,
            bool directorySpecified,
            FakeTokenStore tokenStore)
            : base(workingDirectory, directorySpecified, tokenStore)
        {
            FakeTokenStore = tokenStore;
        }
        
        public FakeTokenStore FakeTokenStore { get; }
    }
}