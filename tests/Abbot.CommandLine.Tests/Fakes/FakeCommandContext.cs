using Serious.IO.CommandLine;
using Serious.IO.CommandLine.Services;
using TestFakes;

namespace UnitTests.Fakes
{
    public class FakeCommandContext : CommandContext
    {
        readonly FakeApiClientFactory _fakeApiClientFactory;

        public FakeCommandContext()
            : this(
                new FakeConsole(),
                new FakeFileSystem(),
                new FakeApiClientFactory())
        {
        }

        FakeCommandContext(
            FakeConsole console,
            FakeFileSystem fileSystem,
            FakeApiClientFactory apiClientFactory)
            : this(console, fileSystem, apiClientFactory, new FakeWorkspaceFactory(fileSystem))
        {
        }

        FakeCommandContext(
            FakeConsole console,
            FakeFileSystem fileSystem,
            FakeApiClientFactory apiClientFactory,
            FakeWorkspaceFactory workspaceFactory)
            : base(console, apiClientFactory, workspaceFactory)
        {
            FakeConsole = console;
            _fakeApiClientFactory = apiClientFactory;
            FakeFileSystem = fileSystem;
        }
        
        public FakeConsole FakeConsole { get; }
        
        public FakeFileSystem FakeFileSystem { get; }

        public FakeApiClient GetFakeApiClient(Workspace workspace) => (_fakeApiClientFactory.Create(workspace) as FakeApiClient)!;
    }
}