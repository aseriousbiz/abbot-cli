using System;
using System.Threading.Tasks;
using Serious.Secrets;

namespace Serious.IO.CommandLine.Services
{
    public class TokenStore : ITokenStore
    {
        const string AbbotApiToken = nameof(AbbotApiToken);
            
        readonly IFileInfo _secretIdFile;
        readonly Func<string, ISecretStore> _secretStoreConstructor;

        /// <summary>
        /// Constructs a token store.
        /// </summary>
        /// <param name="secretIdFile">Contains the secret Id used to look up the token in the <see cref="SecretStore"/>.</param>
        /// <param name="secretStoreConstructor"></param>
        public TokenStore(IFileInfo secretIdFile, Func<string, ISecretStore> secretStoreConstructor)
        {
            _secretIdFile = secretIdFile;
            _secretStoreConstructor = secretStoreConstructor;
        }

        public bool Empty
        {
            get;
            private set;
        }

        /// <summary>
        /// Stores the token in the <see cref="SecretStore"/> for the Workspace and stores the secret Id in the
        /// secret Id file (.abbot/TOKEN).
        /// </summary>
        /// <param name="token">The API token to store.</param>
        public async Task StoreTokenAsync(string token)
        {
            var secretId = await GetSecretIdAsync();
            var secretStore = _secretStoreConstructor(secretId);
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
            var secretId = await GetSecretIdAsync();
            var secretStore = _secretStoreConstructor(secretId);
            await secretStore.LoadAsync();
            var token = secretStore[AbbotApiToken];
            Empty = token is not null;
            return token;
        }
        
        // Retrieves the Secret Id associated with this Abbot Workspace.
        // The Secret Id is used to retrieve the actual Abbot API Token in the SecretStore
        async Task<string> GetSecretIdAsync()
        {
            var secretId = await _secretIdFile.ReadAllTextAsync();
            if (secretId is null)
            {
                secretId = Guid.NewGuid().ToString();
                await _secretIdFile.WriteAllTextAsync(secretId);
            }

            return secretId;
        }
    }
}