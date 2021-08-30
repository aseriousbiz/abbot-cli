using Serious.Abbot.CommandLine.Services;
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
            var factory = new WorkspaceFactory(dir => new FakeDirectoryInfo(dir));
            
            var workspace = factory.GetWorkspace(directory);

            Assert.Equal(expected, workspace.DirectorySpecified);
        }
    }
}
