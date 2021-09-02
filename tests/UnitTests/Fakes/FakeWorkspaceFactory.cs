using System.Collections.Generic;
using Serious.Abbot.CommandLine.Services;

namespace UnitTests.Fakes
{
    public class FakeWorkspaceFactory : IWorkspaceFactory
    {
        readonly Dictionary<string, Workspace> _workspaces = new();
        
        public Workspace GetWorkspace(string? directory)
        {
            return _workspaces.GetOrCreate(
                directory ?? ".",
                dir => new Workspace(new FakeDirectoryInfo(dir ?? "."), dir is not null));
        }
    }
}