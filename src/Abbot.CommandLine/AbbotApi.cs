using System;
using System.Net;
using System.Threading.Tasks;
using Refit;

namespace Serious.Abbot.CommandLine
{
    public static class AbbotApi
    {
        /// <summary>
        /// If the response status is not successful, then this examines the response and writes an appropriate
        /// error message to the console.
        /// </summary>
        /// <param name="response">The response</param>
        /// <typeparam name="T">The response body type</typeparam>
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
