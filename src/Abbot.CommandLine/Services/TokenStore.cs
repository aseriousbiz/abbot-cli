using System;
using System.Threading.Tasks;
using Serious.Secrets;

namespace Serious.IO.CommandLine.Services
{
    public class TokenStore : ITokenStore
    {
        const string AbbotApiToken = nameof(AbbotApiToken);
            
        readonly IFileInfo _secretsIdFile;
        readonly IFileInfo _secretsDirectoryFile;
        readonly Func<string, string?, ISecretStore> _secretStoreConstructor;

        /// <summary>
        /// Constructs a token store.
        /// </summary>
        /// <param name="secretsIdFile">Contains the Id used to look up the set of secrets in the <see cref="SecretStore"/> for this workspace.</param>
        /// <param name="secretsDirectoryFile">If it exists, contains an override directory for where to look for secrets.</param>
        /// <param name="secretStoreConstructor">Used to construct a <see cref="ISecretStore"/>.</param>
        public TokenStore(
            IFileInfo secretsIdFile,
            IFileInfo secretsDirectoryFile,
            Func<string, string?, ISecretStore> secretStoreConstructor)
        {
            _secretsIdFile = secretsIdFile;
            _secretsDirectoryFile = secretsDirectoryFile;
            _secretStoreConstructor = secretStoreConstructor;
        }

        public bool Empty
        {
            get;
            private set;
        }

        /// <summary>
        /// Stores the token in the <see cref="SecretStore"/> for the Workspace and stores the secret Id in the
        /// secret Id file (.abbot/SecretsId).
        /// </summary>
        /// <param name="token">The API token to store.</param>
        public async Task StoreTokenAsync(string token)
        {
            var secretStore = await GetSecretStoreAsync();
            await secretStore.LoadAsync();
            secretStore.SetSecret(AbbotApiToken, token);
            Empty = false;
            await secretStore.SaveAsync();
        }

        /// <summary>
        /// Retrieves the stored token from the <see cref="SecretStore" /> for the current workspace based on
        /// the current secret id.
        /// </summary>
        public async Task<string?> RetrieveTokenAsync()
        {
            var secretStore = await GetSecretStoreAsync();
            await secretStore.LoadAsync();
            var token = secretStore[AbbotApiToken];
            Empty = token is not null;
            return token;
        }

        async Task<ISecretStore> GetSecretStoreAsync()
        {
            var secretsId = await GetSecretsIdAsync();
            var secretDirectoryOverride = await GetSecretsOverrideDirectory();
            var secretStore = _secretStoreConstructor(secretsId, secretDirectoryOverride);
            await secretStore.LoadAsync();
            return secretStore;
        }

        // Retrieves the Secret Id associated with this Abbot Workspace.
        // The Secret Id is used to retrieve the actual Abbot API Token in the SecretStore
        async Task<string> GetSecretsIdAsync()
        {
            var secretsId = await _secretsIdFile.ReadAllTextAsync();
            if (secretsId is null)
            {
                secretsId = Guid.NewGuid().ToString();
                await _secretsIdFile.WriteAllTextAsync(secretsId);
            }

            return secretsId;
        }

        async Task<string?> GetSecretsOverrideDirectory()
        {
            return _secretsDirectoryFile.Exists
                   && await _secretsDirectoryFile.ReadAllTextAsync() is {Length: > 0} directory
                ? directory
                : null;
        }
    }
}