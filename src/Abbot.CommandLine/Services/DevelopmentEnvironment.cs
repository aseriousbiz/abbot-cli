using System.IO;
using System.Threading.Tasks;

namespace Serious.Abbot.CommandLine.Services
{
    public class DevelopmentEnvironment
    {
        readonly DirectoryInfo _metadataDirectory;
        readonly TokenStore _tokenStore;

        public static async Task<DevelopmentEnvironment> EnsureEnvironmentAsync(string directory)
        {
            var environment = GetEnvironment(directory);
            
            var workingDir = environment.WorkingDirectory;
            if (!workingDir.Exists)
            {
                workingDir.Create();
            }

            var metadataDir = environment._metadataDirectory;
            if (!metadataDir.Exists)
            {
                metadataDir.Create();
                metadataDir.Attributes |= FileAttributes.Hidden;
            }

            await environment.EnsureGitIgnoreAsync();
            
            return environment;
        }
        
        public static DevelopmentEnvironment GetEnvironment(string directory)
        {
            if (directory is { Length: 0 })
            {
                directory = ".";
            }
            var workingDir = new DirectoryInfo(directory);
            var metadataDir = new DirectoryInfo(Path.Combine(workingDir.FullName, ".abbot"));
            var environment = new DevelopmentEnvironment(workingDir, metadataDir);
            return environment;
        }

        DevelopmentEnvironment(DirectoryInfo workingDirectory, DirectoryInfo metadataDirectory)
        {
            WorkingDirectory = workingDirectory;
            _metadataDirectory = metadataDirectory;
            var tokenFile = new FileInfo(Path.Combine(_metadataDirectory.FullName, "TOKEN"));
            _tokenStore = new TokenStore(new TokenProtector(), tokenFile);
        }

        public bool IsInitialized => _metadataDirectory.Exists;
        
        public bool IsAuthenticated => !_tokenStore.Empty;

        public DirectoryInfo WorkingDirectory { get; }

        /// <summary>
        /// The stored API Key token.
        /// </summary>
        public Task<string?> GetTokenAsync()
        {
            return _tokenStore.RetrieveTokenAsync();
        }
        
        /// <summary>
        /// The stored API Key token.
        /// </summary>
        /// <param name="token">The token to store</param>
        public Task SetTokenAsync(string token)
        {
            return _tokenStore.StoreTokenAsync(token);
        }

        async Task EnsureGitIgnoreAsync()
        {
            var gitignore = new FileInfo(Path.Combine(_metadataDirectory.FullName, ".gitignore"));
            if (gitignore.Exists)
            {
                return;
            }

            await using var stream = gitignore.OpenWrite();
            await using var writer = new StreamWriter(stream);
            await writer.WriteLineAsync("*");
        }
    }
}