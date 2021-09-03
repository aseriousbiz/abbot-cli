using System;
using Serious.IO.CommandLine.Services;
using Serious.Secrets;

namespace Serious.IO.CommandLine
{
    /// <summary>
    /// Constructors we use to build up dependencies in production.
    /// </summary>
    public static class Constructors
    {
        public static Func<string, ISecretStore> CreateSecretStoreConstructor(
            IFileSystem fileSystem,
            ISecretProtector protector)
        {
            return secretsId => new SecretStore(secretsId, fileSystem, protector, PathHelper.GetSecretsPathFromSecretsId);
        }

        public static Func<IFileInfo, ITokenStore> CreateTokenStoreConstructor(Func<string, ISecretStore> secretStoreConstructor)
        {
            return secretsIdFile => new TokenStore(secretsIdFile, secretStoreConstructor);
        }

        public static Func<IDirectoryInfo, bool, ITokenStore, Workspace> WorkspaceConstructor()
        {
            return (dir, specified, tokenStore) => new Workspace(dir, specified, tokenStore);
        }
    }
}