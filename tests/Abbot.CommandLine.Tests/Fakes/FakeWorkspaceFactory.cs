using Serious.IO.CommandLine.Services;
using TestFakes;

namespace UnitTests.Fakes
{
    public class FakeWorkspaceFactory : WorkspaceFactory
    {
        public FakeWorkspaceFactory(FakeFileSystem fileSystem)
            : base(fileSystem,
                FakeConstructors.TokenStoreConstructor(fileSystem),
                FakeConstructors.WorkspaceConstructor())
        {
            FakeFileSystem = fileSystem;
        }

        public FakeFileSystem FakeFileSystem { get; }
    }
}