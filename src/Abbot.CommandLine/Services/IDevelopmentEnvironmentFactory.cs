namespace Serious.Abbot.CommandLine.Services
{
    public interface IDevelopmentEnvironmentFactory
    {
        /// <summary>
        /// Creates an instance of a <see cref="DevelopmentEnvironment"/> pointing to the specified directory.
        /// Note that this doesn't create the actual environment. EnsureAsync has to be called next.
        /// </summary>
        /// <param name="directory">Path to the development environment working directory.</param>
        /// <returns></returns>
        DevelopmentEnvironment GetDevelopmentEnvironment(string directory);
    }
}