using System.Collections.Generic;
using Serious.Abbot.CommandLine;
using Serious.Abbot.CommandLine.Services;

namespace UnitTests.Fakes
{
    public class FakeApiClientFactory : IApiClientFactory
    {
        readonly Dictionary<string, IAbbotApi> _clients = new();
        public IAbbotApi Create(Workspace workspace)
        {
            return _clients.GetOrCreate(workspace.WorkingDirectory.FullName, _ => new FakeApiClient());
        }
    }
}