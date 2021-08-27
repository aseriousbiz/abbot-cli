using System.Threading.Tasks;
using Serious.Abbot.CommandLine.Editors;
using Serious.Abbot.CommandLine.IO;

namespace Serious.Abbot.CommandLine.Services
{
    /// <summary>
    /// Represents an initialized Abbot Skills Folder. This is the root folder where skill development occurs. Each
    /// sub-folder of an Abbot Skills Folder represents a skill to be edited.
    /// </summary>
    public class DevelopmentEnvironment
    {
        readonly IDirectoryInfo _metadataDirectory;
        readonly TokenStore _tokenStore;

        /// <summary>
        /// Constructs an instance of a Development Environment.
        /// </summary>
        /// <param name="workingDirectory">The working directory.</param>
        /// <param name="directorySpecified">Whether the working directory was specified or is the current directory.</param>
        public DevelopmentEnvironment(IDirectoryInfo workingDirectory, bool directorySpecified)
            : this(workingDirectory, workingDirectory.GetSubdirectory(".abbot"), directorySpecified)
        {
        }

        DevelopmentEnvironment(IDirectoryInfo workingDirectory, IDirectoryInfo metadataDirectory, bool directorySpecified)
            : this(
                workingDirectory,
                metadataDirectory,
                new TokenStore(new TokenProtector(), metadataDirectory.GetFile("TOKEN")),
                directorySpecified)
        {
        }

        DevelopmentEnvironment(
            IDirectoryInfo workingDirectory,
            IDirectoryInfo metadataDirectory,
            TokenStore tokenStore,
            bool directorySpecified)
        {
            WorkingDirectory = workingDirectory;
            _metadataDirectory = metadataDirectory;
            _tokenStore = tokenStore;
            DirectorySpecified = directorySpecified;
        }

        public async Task EnsureAsync()
        {
            WorkingDirectory.Create();
            _metadataDirectory.Create();
            _metadataDirectory.Hide();
            
            await EnsureGitIgnoreAsync();
            await EnsureReferencesFileAsync();
            await EnsureOmniSharpConfigAsync();
        }
        
        public bool Exists => WorkingDirectory.Exists;

        public bool DirectorySpecified { get; }
        
        /// <summary>
        /// Get a skill environment for the specified skill.
        /// </summary>
        /// <param name="skill">The name of the skill.</param>
        public SkillEnvironment GetSkillEnvironment(string skill)
        {
            var skillDirectory = WorkingDirectory.GetSubdirectory(skill);
            return new SkillEnvironment(skillDirectory);
        }

        public bool IsInitialized => _metadataDirectory.Exists;
        
        public bool IsAuthenticated => !_tokenStore.Empty;

        public IDirectoryInfo WorkingDirectory { get; }

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
            var gitignore = _metadataDirectory.GetFile(".gitignore");
            if (gitignore.Exists)
            {
                return;
            }

            await gitignore.WriteAllTextAsync("*");
        }
        
        Task EnsureReferencesFileAsync()
        {
            return Omnisharp.WriteRspFileAsync(_metadataDirectory);
        }
        
        Task EnsureOmniSharpConfigAsync()
        {
            return Omnisharp.WriteConfigFileAsync(WorkingDirectory, string.Empty);
        }
    }
}