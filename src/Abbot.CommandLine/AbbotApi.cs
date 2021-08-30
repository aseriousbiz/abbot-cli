using System;
using System.Net;
using System.Threading.Tasks;
using Refit;
using Serious.Abbot.CommandLine.Commands;
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

        public static IAbbotApi CreateInstance(Workspace workspace)
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

        /// <summary>
        /// If the response status is not successful, then this examines the response and writes an appropriate
        /// error message to the console.
        /// </summary>
        /// <param name="response">The response</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The error code to return.</returns>
        public static async Task<int> HandleUnsuccessfulResponseAsync<T>(this ApiResponse<T> response)
        {
            if (response.Error is ValidationApiException {Content: {Detail: {Length: > 0}}} exception)
            {
                await Console.Error.WriteLineAsync(exception.Content.Detail);
                return 1;
            }

            var message = response.StatusCode switch
            {
                HttpStatusCode.NotFound => "Could not find a skill of that name.",
                HttpStatusCode.InternalServerError => "An error occurred on the server. Contact support@aseriousbusiness.com to learn more. It's their fault.",
                HttpStatusCode.Unauthorized => "The API Key you provided is not valid or expired. Run \"abbot auth\" to authenticate again.",
                HttpStatusCode.Forbidden => "You do not have permission to edit that skill. Contact your administrators to request permission.",
                _ => $"Received a {response.StatusCode} response from the server"
            };
            await Console.Error.WriteLineAsync(message);
            return 1; // TODO: We may want to have appropriate error codes for each condition in the future.
        }
    }
}
