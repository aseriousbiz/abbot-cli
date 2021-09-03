using System;
using System.IO;
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
            return secretsId =>
            {
                var secretsFilePath = PathHelper.GetSecretsPathFromSecretsId(secretsId);
                var secretsFile = new Lazy<IFileInfo>(() =>
                {
                    var secretDir = Path.GetDirectoryName(secretsFilePath)!;
                    fileSystem.CreateDirectory(secretDir);
                    return new FileInfoWrapper(secretsFilePath);
                });
                return new SecretStore(secretsFile, protector);
            };
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