namespace Serious.Abbot.CommandLine.Services
{
    /// <summary>
    /// Used to create Abbot API clients.
    /// </summary>
    public interface IApiClientFactory
    {
        /// <summary>
        /// Creates the Abbot API client using the API token stored in the workspace.
        /// </summary>
        /// <param name="workspace">The workspace</param>
        IAbbotApi Create(Workspace workspace);
    }
}