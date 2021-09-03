using System.Threading.Tasks;

namespace Serious.Secrets
{
    /// <summary>
    /// Stores secrets for an application. The application just needs to store a SecretId used to identify
    /// the secret store.
    /// </summary>
    public interface ISecretStore
    {
        /// <summary>
        /// Loads the secrets from the secret store.
        /// </summary>
        Task LoadAsync();
        
        /// <summary>
        /// Saves the set of secrets in the secret store.
        /// </summary>
        /// <returns></returns>
        Task SaveAsync();
        
        /// <summary>
        /// Retrieves a secret with the specified key.
        /// </summary>
        /// <param name="key">The key for the secret.</param>
        string? this[string key] { get; }

        /// <summary>
        /// Sets a secret associated with the specified key. Not saved until <see cref="SaveAsync"/> is called.
        /// </summary>
        /// <param name="key">The key for the secret.</param>
        /// <param name="value">The secret value.</param>
        void SetSecret(string key, string value);
    }
}