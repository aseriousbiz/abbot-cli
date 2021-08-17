using Refit;
using Serious.Abbot.CommandLine.Services;

namespace Serious.Abbot.CommandLine
{
    public static class AbbotApi
    {
#if DEBUG
        const string ApiHost = "https://localhost:4979/";
#else
        const string ApiHost = "https://api.ab.bot/";
#endif

        public static IAbbotApi CreateInstance(DevelopmentEnvironment environment) => RestService.For<IAbbotApi>(ApiHost, new RefitSettings()
        {
            AuthorizationHeaderValueGetter = async () => await environment.GetTokenAsync() ?? string.Empty
        });
    }
}