using System.Threading.Tasks;
using Serious.Secrets;

namespace Serious.IO.CommandLine.Services
{
    public interface ITokenStore
    {
        /// <summary>
        /// Stores the token in the <see cref="SecretStore"/> for the Workspace and stores the secret Id in the
        /// secret Id file (.abbot/SecretsId).
        /// </summary>
        /// <param name="token">The API token to store.</param>
        Task StoreTokenAsync(string token);

        /// <summary>
        /// Retrieves the stored token from the <see cref="SecretStore" /> for the current workspace based on
        /// the current secret id.
        /// </summary>
        Task<string?> RetrieveTokenAsync();
        
        /// <summary>
        /// Whether or not a token has been stored.
        /// </summary>
        bool Empty { get; }

        /// <summary>
        /// Uses the specified directory to store secrets instead of the default location.
        /// </summary>
        /// <param name="path">A path to a directory where secrets will be stored.</param>
        Task SetSecretsDirectoryAsync(string path);
    }
}