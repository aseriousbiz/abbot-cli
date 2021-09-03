using Refit;
using Serious.IO.CommandLine.Commands;

namespace Serious.IO.CommandLine.Services
{
    public class ApiClientFactory : IApiClientFactory
    {
#if DEBUG
        const string ApiHost = "https://localhost:4979/";
#else
        const string ApiHost = "https://api.ab.bot/";
#endif

        public IAbbotApi Create(Workspace workspace)
        {
            var settings = new RefitSettings
            {
                AuthorizationHeaderValueGetter = async () => await workspace.GetTokenAsync() ?? string.Empty,
            };
#pragma warning disable CA2000
            var httpClient = RestService.CreateHttpClient(ApiHost, settings);
#pragma warning restore CA2000
            var version = StatusCommand.GetVersion();
            httpClient.DefaultRequestHeaders.Add("X-Client-Version", version);
            httpClient.DefaultRequestHeaders.Add("User-Agent", $"abbot-cli ({version})");
            return RestService.For<IAbbotApi>(httpClient);
        }
    }
}