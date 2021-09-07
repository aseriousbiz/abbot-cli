using System.Threading.Tasks;
using Serious.IO.CommandLine.Editors;

namespace Serious.IO.CommandLine.Services
{
    /// <summary>
    /// Represents an initialized Abbot Workspace. This is the root folder where skill development occurs. Each
    /// sub-folder of an Abbot Workspace represents a skill to be edited.
    /// </summary>
    public class Workspace
    {
        readonly IDirectoryInfo _metadataDirectory;
        readonly ITokenStore _tokenStore;

        /// <summary>
        /// Constructs an instance of an Abbot Workspace.
        /// </summary>
        /// <param name="workingDirectory">The working directory.</param>
        /// <param name="directorySpecified">Whether the working directory was specified or is the current directory.</param>
        /// <param name="tokenStore"></param>
        public Workspace(IDirectoryInfo workingDirectory, bool directorySpecified, ITokenStore tokenStore)
        {
            WorkingDirectory = workingDirectory;
            _metadataDirectory = workingDirectory.GetSubdirectory(".abbot");
            _tokenStore = tokenStore;
            DirectorySpecified = directorySpecified;
        }

        public async Task EnsureAsync(string? secretsDirectoryPath)
        {
            WorkingDirectory.Create();
            _metadataDirectory.Create();
            _metadataDirectory.Hide();
            
            await EnsureReferencesFileAsync();
            await EnsureOmniSharpConfigAsync();

            if (secretsDirectoryPath is {Length: > 0})
            {
                await _tokenStore.SetSecretsDirectoryAsync(secretsDirectoryPath);
            }
        }
        
        public bool Exists => WorkingDirectory.Exists;

        public bool DirectorySpecified { get; }
        
        /// <summary>
        /// Get a <see cref="GetSkillWorkspace"/> for the specified skill.
        /// </summary>
        /// <param name="skill">The name of the skill.</param>
        public SkillWorkspace GetSkillWorkspace(string skill)
        {
            var skillDirectory = WorkingDirectory.GetSubdirectory(skill);
            return new SkillWorkspace(skillDirectory);
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
        
        Task EnsureReferencesFileAsync()
        {
            return OmniSharpHelpers.WriteRspFileAsync(_metadataDirectory);
        }
        
        Task EnsureOmniSharpConfigAsync()
        {
            return OmniSharpHelpers.WriteConfigFileAsync(WorkingDirectory, string.Empty);
        }
    }
}