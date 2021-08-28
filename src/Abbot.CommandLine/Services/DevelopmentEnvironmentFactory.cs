using System;
using Serious.Abbot.CommandLine.IO;

namespace Serious.Abbot.CommandLine.Services
{
    public class DevelopmentEnvironmentFactory : IDevelopmentEnvironmentFactory
    {
        readonly Func<string, IDirectoryInfo> _directoryFactory;

        public DevelopmentEnvironmentFactory() : this(directory => new DirectoryInfoWrapper(directory))
        {
        }

        public DevelopmentEnvironmentFactory(Func<string, IDirectoryInfo> directoryFactory)
        {
            _directoryFactory = directoryFactory;
        }

        public DevelopmentEnvironment GetDevelopmentEnvironment(string? directory)
        {
            var directorySpecified = directory is { Length: > 0 };
            var workingDirectory = _directoryFactory(directory ?? ".");
            return new DevelopmentEnvironment(workingDirectory, directorySpecified);
        }
    }
}