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
                new FakeDataProtector(),
                new FakeApiClientFactory())
        {
        }

        FakeCommandContext(
            FakeConsole console,
            FakeFileSystem fileSystem,
            FakeDataProtector dataProtector,
            FakeApiClientFactory apiClientFactory)
            : this(console, apiClientFactory, new FakeWorkspaceFactory(fileSystem, dataProtector))
        {
        }

        FakeCommandContext(
            FakeConsole console,
            FakeApiClientFactory apiClientFactory,
            FakeWorkspaceFactory workspaceFactory)
            : base(console, apiClientFactory, workspaceFactory)
        {
            FakeConsole = console;
            FakeFileSystem = workspaceFactory.FakeFileSystem;
            FakeDataProtector = workspaceFactory.FakeDataProtector;
            _fakeApiClientFactory = apiClientFactory;
            FakeWorkspaceFactory = workspaceFactory;
        }
        
        public FakeFileSystem FakeFileSystem { get; }
        public FakeConsole FakeConsole { get; }
        public FakeDataProtector FakeDataProtector { get; }
        public FakeWorkspaceFactory FakeWorkspaceFactory { get; }
        public FakeApiClient GetFakeApiClient(Workspace workspace) => (_fakeApiClientFactory.Create(workspace) as FakeApiClient)!;
    }
}