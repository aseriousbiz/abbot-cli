using Serious.Abbot.CommandLine;
using Serious.Abbot.CommandLine.Services;

namespace UnitTests.Fakes
{
    public class FakeCommandContext : CommandContext
    {
        readonly FakeApiClientFactory _fakeApiClientFactory;
        
        public FakeCommandContext()
            : this(new FakeConsole(), new FakeWorkspaceFactory(), new FakeApiClientFactory())
        {
        }

        FakeCommandContext(
            FakeConsole console,
            IWorkspaceFactory workspaceFactory,
            FakeApiClientFactory apiClientFactory)
            : base(
                console,
                workspaceFactory,
                apiClientFactory)
        {
            FakeConsole = console;
            _fakeApiClientFactory = apiClientFactory;
        }
        
        public FakeConsole FakeConsole { get; }

        public FakeApiClient GetFakeApiClient(Workspace workspace) => (_fakeApiClientFactory.Create(workspace) as FakeApiClient)!;
    }
}