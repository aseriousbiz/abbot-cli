using Serious.Abbot.CommandLine.IO;

namespace Serious.Abbot.CommandLine.Services
{
    public class DevelopmentEnvironmentFactory : IDevelopmentEnvironmentFactory
    {
        public DevelopmentEnvironment GetDevelopmentEnvironment(string directory)
        {
            var workingDirectory = new DirectoryInfoWrapper(directory);
            return new DevelopmentEnvironment(workingDirectory);
        }
    }
}