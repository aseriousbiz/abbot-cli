using System;
using System.IO;
using Microsoft.AspNetCore.DataProtection;
using Serious.IO.CommandLine.Services;
using Serious.Secrets;

namespace Serious.IO.CommandLine
{
    /// <summary>
    /// Constructors we use to build up dependencies in production.
    /// </summary>
    /// <remarks>
    /// Yes, this should probably be handled by a Dependency Injection Framework, but I'm waiting until the last
    /// responsible moment to introduce one. Maybe even a little past the last responsible moment.
    /// </remarks>
    public static class Constructors
    {
        /// <summary>
        /// Creates a command context for use in all our commands.
        /// </summary>
        /// <returns></returns>
        public static ICommandContext CreateCommandContext()
        {
            var fileSystem = new FileSystem();
            var dataProtector = DataProtectionProvider
                .Create("abbot")
                .CreateProtector("Store Abbot Workspace Secrets");
            var secretProtector = new SecretProtector(dataProtector);
            var apiClientFactory = new ApiClientFactory();
            var console = new ExtendedSystemConsole();
            var tokenConstructor =
                TokenStoreConstructor(
                    SecretStoreConstructor(fileSystem, secretProtector, PathHelper.GetSecretsPathFromSecretsId));
            var workspaceConstructor = WorkspaceConstructor();
            var workspaceFactory = new WorkspaceFactory(fileSystem, tokenConstructor, workspaceConstructor);
            
            return new CommandContext(console, apiClientFactory, workspaceFactory);
        }

        /// <summary>
        /// Creates a function used to create <see cref="ISecretStore" /> instances.
        /// </summary>
        /// <param name="fileSystem">The filesystem.</param>
        /// <param name="protector">The secret protector.</param>
        /// <param name="defaultSecretsFilePathLookup">The default logic for locating the secrets file.</param>
        public static Func<string, string?, ISecretStore> SecretStoreConstructor(
            IFileSystem fileSystem,
            ISecretProtector protector,
            Func<string, string> defaultSecretsFilePathLookup)
        {
            return (secretsId, secretsDirectory) =>
            {
                var secretsFilePath = secretsDirectory is null
                    ? defaultSecretsFilePathLookup(secretsId)
                    : Path.Combine(secretsDirectory, secretsId);
                var secretsFile = new Lazy<IFileInfo>(() =>
                {
                    var secretDir = Path.GetDirectoryName(secretsFilePath)!;
                    fileSystem.CreateDirectory(secretDir);
                    return fileSystem.GetFile(secretsFilePath);
                });
                return new SecretStore(secretsFile, protector);
            };
        }

        static Func<IFileInfo, IFileInfo, ITokenStore> TokenStoreConstructor(Func<string, string?, ISecretStore> secretStoreConstructor)
        {
            return (secretsIdFile, secretsDirectoryFile) => new TokenStore(secretsIdFile, secretsDirectoryFile, secretStoreConstructor);
        }

        static Func<IDirectoryInfo, bool, ITokenStore, Workspace> WorkspaceConstructor()
        {
            return (dir, specified, tokenStore) => new Workspace(dir, specified, tokenStore);
        }
    }
}