using System;
using Serious.IO;
using Serious.IO.CommandLine;
using Serious.IO.CommandLine.Services;
using Serious.Secrets;
using TestFakes;

namespace UnitTests.Fakes
{
    public class FakeConstructors
    {
        public static Func<IFileInfo, IFileInfo, ITokenStore> TokenStoreConstructor(
            Func<string, string?, ISecretStore> secretStoreConstructor)
        {
            return (secretsIdFile, secretsDirectoryFile) => new FakeTokenStore(
                (FakeFileInfo)secretsIdFile,
                (FakeFileInfo)secretsDirectoryFile,
                secretStoreConstructor);
        }

        public static Func<string, string?, ISecretStore> SecretStoreConstructor(
            FakeFileSystem fileSystem,
            ISecretProtector protector) =>
                Constructors.SecretStoreConstructor(fileSystem, protector, sid => $".abbot/secrets/{sid}");

        public static Func<IFileInfo, IFileInfo, ITokenStore> TokenStoreConstructor(FakeFileSystem fileSystem)
        {
            var secretStoreConstructor = SecretStoreConstructor(
                fileSystem,
                new FakeSecretProtector());
            return TokenStoreConstructor(secretStoreConstructor);
        }

        public static Func<IDirectoryInfo, bool, ITokenStore, Workspace> WorkspaceConstructor()
        {
            return (dir, specified, tokenStore) => new FakeWorkspace(dir, specified, (FakeTokenStore)tokenStore);
        }
    }
}