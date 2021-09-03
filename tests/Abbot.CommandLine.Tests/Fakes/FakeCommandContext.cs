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
            : this(console, apiClientFactory, new FakeWorkspaceFactory(fileSystem))
        {
        }

        FakeCommandContext(
            FakeConsole console,
            FakeApiClientFactory apiClientFactory,
            FakeWorkspaceFactory workspaceFactory)
            : base(console, apiClientFactory, workspaceFactory)
        {
            FakeConsole = console;
            _fakeApiClientFactory = apiClientFactory;
        }
        
        public FakeConsole FakeConsole { get; }

        public FakeApiClient GetFakeApiClient(Workspace workspace) => (_fakeApiClientFactory.Create(workspace) as FakeApiClient)!;
    }
}