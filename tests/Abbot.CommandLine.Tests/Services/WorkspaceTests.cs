using System.Threading.Tasks;
using Serious.IO.CommandLine.Services;
using TestFakes;
using UnitTests.Fakes;
using Xunit;

public class WorkspaceTests
{
    public class TheConstructor
    {
        [Fact]
        public void CreatesWorkspaceInstanceButNothingOnDisk()
        {
            var fileSystem = new FakeFileSystem();
            var tokenConstructor = FakeConstructors.TokenStoreConstructor(fileSystem);
            var secretsIdFile = fileSystem.GetFile("./abbot/SecretsId");
            var secretsDirectoryFile = fileSystem.GetFile("./abbot/SecretsDirectory");
            var tokenStore = tokenConstructor(secretsIdFile, secretsDirectoryFile);
            var workspace = new Workspace(
                fileSystem.GetDirectory("."),
                true,
                tokenStore);
            
            Assert.False(workspace.Exists);
            Assert.True(workspace.DirectorySpecified);
        }
    }

    public class TheEnsureAsyncMethod
    {
        [Fact]
        public async Task CreatesFoldersAndFilesForWorkspace()
        {
            var fileSystem = new FakeFileSystem();
            var workingDirectory = fileSystem.GetDirectory(".");
            var tokenConstructor = FakeConstructors.TokenStoreConstructor(fileSystem);
            var secretsIdFile = fileSystem.GetFile("./abbot/SecretsId");
            var secretsDirectoryFile = fileSystem.GetFile("./abbot/SecretsDirectory");
            var tokenStore = tokenConstructor(secretsIdFile, secretsDirectoryFile);

            var workspace = new Workspace(workingDirectory, true, tokenStore);

            await workspace.EnsureAsync();
            
            Assert.True(workingDirectory.Exists);
            var omnisharpConfig = workingDirectory.GetFile("omnisharp.json");
            Assert.True(omnisharpConfig.Exists);
            Assert.Equal("./omnisharp.json", omnisharpConfig.FullName);
            var metadataDirectory = workingDirectory.GetSubdirectory(".abbot");
            Assert.True(metadataDirectory is FakeDirectoryInfo {Hidden: true});
            Assert.Equal("./.abbot", metadataDirectory.FullName);
            Assert.True(metadataDirectory.Exists);
            var references = metadataDirectory.GetFile("references.rsp");
            Assert.True(references.Exists);
            Assert.Equal("./.abbot/references.rsp", references.FullName);
        }
    }
}
