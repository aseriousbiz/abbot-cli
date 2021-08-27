using System.IO;
using System.Threading.Tasks;
using Serious.Abbot.CommandLine.Editors;

namespace Serious.Abbot.CommandLine.Services
{
    /// <summary>
    /// Represents an initialized Abbot Skills Folder. This is the root folder where skill development occurs. Each
    /// sub-folder of an Abbot Skills Folder represents a skill to be edited.
    /// </summary>
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
            await environment.EnsureReferencesFileAsync();
            await environment.EnsureOmniSharpConfigAsync();
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

        /// <summary>
        /// Get a skill environment for the specified skill.
        /// </summary>
        /// <param name="skill">The name of the skill.</param>
        public SkillEnvironment GetSkillEnvironment(string skill)
        {
            return new SkillEnvironment(this, skill);
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

            await FileHelpers.WriteAllTextAsync(gitignore.FullName, "*");
        }
        
        Task EnsureReferencesFileAsync()
        {
            return Omnisharp.WriteRspFileAsync(_metadataDirectory.FullName);
        }
        
        Task EnsureOmniSharpConfigAsync()
        {
            return Omnisharp.WriteConfigFileAsync(WorkingDirectory.FullName, ".abbot/references.rsp");
        }
    }
}