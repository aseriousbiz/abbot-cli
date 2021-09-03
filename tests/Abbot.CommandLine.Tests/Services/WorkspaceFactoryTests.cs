using Serious.IO.CommandLine.Services;
using TestFakes;
using UnitTests.Fakes;
using Xunit;

public class WorkspaceFactoryTests
{
    public class TheGetWorkspaceMethod
    {
        [Theory]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData(".", true)]
        public void SetsDirectorySpecifiedWhenDirectoryNullOrEmpty(string? directory, bool expected)
        {
            var fileSystem = new FakeFileSystem();
            var secretStoreConstructor =
                FakeConstructors.SecretStoreConstructor(fileSystem, new FakeSecretProtector());
            var tokenConstructor = FakeConstructors.TokenStoreConstructor(secretStoreConstructor);
            var workspaceConstructor = FakeConstructors.WorkspaceConstructor();
            var factory = new WorkspaceFactory(fileSystem, tokenConstructor, workspaceConstructor);
            
            var workspace = factory.GetWorkspace(directory);

            Assert.Equal(expected, workspace.DirectorySpecified);
        }
    }
}
