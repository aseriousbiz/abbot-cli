using System.Threading.Tasks;
using Serious.Abbot.CommandLine.Services;
using UnitTests.Fakes;
using Xunit;

public class WorkspaceTests
{
    public class TheConstructor
    {
        [Fact]
        public void CreatesWorkspaceInstanceButNothingOnDisk()
        {
            var workspace = new Workspace(new FakeDirectoryInfo("."), true);
            
            Assert.False(workspace.Exists);
            Assert.True(workspace.DirectorySpecified);
        }
    }

    public class TheEnsureAsyncMethod
    {
        [Fact]
        public async Task CreatesFoldersAndFilesForWorkspace()
        {
            var workingDirectory = new FakeDirectoryInfo(".");
            var workspace = new Workspace(workingDirectory, true);

            await workspace.EnsureAsync();
            
            Assert.True(workingDirectory.Exists);
            var omnisharpConfig = workingDirectory.GetFile("omnisharp.json");
            Assert.True(omnisharpConfig.Exists);
            Assert.Equal("./omnisharp.json", omnisharpConfig.FullName);
            var metadataDirectory = workingDirectory.GetSubdirectory(".abbot");
            Assert.True(metadataDirectory is FakeDirectoryInfo {Hidden: true});
            Assert.Equal("./.abbot", metadataDirectory.FullName);
            Assert.True(metadataDirectory.Exists);
            var gitignore = metadataDirectory.GetFile(".gitignore");
            Assert.True(gitignore.Exists);
            Assert.Equal("./.abbot/.gitignore", gitignore.FullName);
            var references = metadataDirectory.GetFile("references.rsp");
            Assert.True(references.Exists);
            Assert.Equal("./.abbot/references.rsp", references.FullName);
        }
    }
}
