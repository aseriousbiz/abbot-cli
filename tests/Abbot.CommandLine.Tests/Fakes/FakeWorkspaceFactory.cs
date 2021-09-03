using Serious.IO.CommandLine.Services;
using TestFakes;

namespace UnitTests.Fakes
{
    public class FakeWorkspaceFactory : WorkspaceFactory
    {
        public FakeWorkspaceFactory(FakeFileSystem fileSystem, FakeDataProtector dataProtector)
            : base(
                fileSystem,
                secretFile => new FakeTokenStore((FakeFileInfo)secretFile, FakeConstructors.SecretStoreConstructor(fileSystem, new FakeSecretProtector())),
                (dir, specified, tokenStore) => new FakeWorkspace(dir, specified, (FakeTokenStore)tokenStore))
        {
            FakeFileSystem = fileSystem;
            FakeDataProtector = dataProtector;
        }

        public FakeFileSystem FakeFileSystem { get; }
        
        public FakeDataProtector FakeDataProtector { get; }
    }
}