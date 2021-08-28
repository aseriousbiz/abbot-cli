using Serious.Abbot.CommandLine.Services;
using UnitTests.Fakes;
using Xunit;

public class DevelopmentEnvironmentFactoryTests
{
    public class TheGetDevelopmentEnvironmentMethod
    {
        [Theory]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData(".", true)]
        public void SetsDirectorySpecifiedWhenDirectoryNullOrEmpty(string? directory, bool expected)
        {
            var factory = new DevelopmentEnvironmentFactory(dir => new FakeDirectoryInfo(dir));
            
            var environment = factory.GetDevelopmentEnvironment(directory);

            Assert.Equal(expected, environment.DirectorySpecified);
        }
    }
}
