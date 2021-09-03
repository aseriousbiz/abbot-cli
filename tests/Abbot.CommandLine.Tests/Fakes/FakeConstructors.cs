using System;
using Serious.IO;
using Serious.IO.CommandLine.Services;
using Serious.Secrets;
using TestFakes;

namespace UnitTests.Fakes
{
    public class FakeConstructors
    {
        public static Func<string, ISecretStore> SecretStoreConstructor(
            FakeFileSystem fileSystem,
            FakeSecretProtector protector)
        {
            return secretsId => new FakeSecretStore(secretsId, fileSystem, protector, secretId => $"~/.abbot/secrets/{secretId}");
        }

        public static Func<IFileInfo, ITokenStore> TokenStoreConstructor(Func<string, ISecretStore> secretStoreConstructor)
        {
            return secretsIdFile => new FakeTokenStore((FakeFileInfo)secretsIdFile, secretStoreConstructor);
        }

        public static Func<IFileInfo, ITokenStore> TokenStoreConstructor(FakeFileSystem fileSystem)
        {
            var secretStoreConstructor = SecretStoreConstructor(fileSystem, new FakeSecretProtector());
            return TokenStoreConstructor(secretStoreConstructor);
        }

        public static Func<IDirectoryInfo, bool, ITokenStore, Workspace> WorkspaceConstructor()
        {
            return (dir, specified, tokenStore) => new FakeWorkspace(dir, specified, (FakeTokenStore)tokenStore);
        }
    }
}