using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Serious.Abbot.CommandLine.Editors;
using Serious.Abbot.Entities;
using Serious.Abbot.Messages;

namespace Serious.Abbot.CommandLine.Services
{
    public class SkillEnvironment
    {
        readonly string _skill;
        readonly string _skillDirectory;
        readonly string _skillMetaDirectory;
        readonly string _concurrencyFilePath;

        public SkillEnvironment(DevelopmentEnvironment developmentEnvironment, string skill)
        {
            _skill = skill;
            _skillDirectory = Path.Combine(developmentEnvironment.WorkingDirectory.FullName, skill);
            _skillMetaDirectory = Path.Combine(_skillDirectory, ".meta");
            _concurrencyFilePath = Path.Combine(_skillMetaDirectory, ".concurrency");
        }

        public bool Exists => Directory.Exists(_skillDirectory);

        public string WorkingDirectory => _skillDirectory;

        /// <summary>
        /// Reads the concurrency file for the skill.
        /// </summary>
        /// <returns></returns>
        public Task<string> ReadConcurrencyFileAsync()
        {
            var concurrencyFile = new FileInfo(_concurrencyFilePath);
            return concurrencyFile.Exists
                ? File.ReadAllTextAsync(_concurrencyFilePath)
                : Task.FromResult(string.Empty);
        }

        /// <summary>
        /// Writes the specified code hash to the concurrency file.
        /// </summary>
        /// <param name="codeHash">The hash of the code</param>
        public async Task WriteConcurrencyFileAsync(string codeHash)
        {
            var file = await FileHelpers.WriteAllTextAsync(_concurrencyFilePath, codeHash);
            file.HideFile();
        }

        public Task WriteOmniSharpConfigFileAsync()
        {
            return Omnisharp.WriteConfigFileAsync(_skillDirectory, "../.abbot/references.rsp");
        }
        
        public Task WriteGlobalsCsxFileAsync()
        {
            return Omnisharp.WriteGlobalsCsxFileAsync(_skillMetaDirectory);
        }

        /// <summary>
        /// Creates a <see cref="SkillEnvironment"/> based on the <see cref="SkillGetResponse" />
        /// </summary>
        /// <returns></returns>
        public async Task CreateAsync(SkillGetResponse skillInfo)
        {
            Directory.CreateDirectory(_skillDirectory); // noop if directory already exists.
            Directory.CreateDirectory(_skillMetaDirectory); // noop if directory already exists.

            await WriteConcurrencyFileAsync(skillInfo.CodeHash);
            await WriteCodeAsync(skillInfo.Code, skillInfo.Language);
            
            if (skillInfo.Language is CodeLanguage.CSharp)
            {
                await WriteGlobalsCsxFileAsync();
            }
        }

        public Task WriteCodeAsync(string code, CodeLanguage language)
        {
            var codeFilePath = GetCodeFilePath(language);
            var contents = language is CodeLanguage.CSharp
                ? Omnisharp.EnsureGlobalsDirective(code)
                : code;
            return FileHelpers.WriteAllTextAsync(codeFilePath, contents);
        }
        
        /// <summary>
        /// Returns true if the local code for the skill has local changes.
        /// </summary>
        /// <param name="language">The language for the skill</param>
        public async Task<bool> HasLocalChangesAsync(CodeLanguage language)
        {
            var codeFilePath = GetCodeFilePath(language);
            if (!File.Exists(codeFilePath))
            {
                return false;
            }
            
            var existingCode = await File.ReadAllTextAsync(codeFilePath);
            existingCode = Omnisharp.RemoveGlobalsDirective(existingCode);

            var existingCodeHash = await ReadConcurrencyFileAsync();

            var codeHash = ComputeSHA1Hash(existingCode);
            return existingCodeHash != codeHash;
        }

        /// <summary>
        /// Retrieve the path to the code file when we don't know the language.
        /// </summary>
        public string? GetCodeFilePath()
        {
            var directory = new DirectoryInfo(WorkingDirectory);
            return directory.GetFiles($"{_skill}.*")?.SingleOrDefault()?.FullName;
        }

        string GetCodeFilePath(CodeLanguage language)
        {
            var extension = language.GetFileExtension();
            return Path.Combine(_skillDirectory, $"{_skill}.{extension}");
        }

        static string ComputeSHA1Hash(string value)
        {
            // Using this for checksums
#pragma warning disable CA5350
            using var sha1 = new SHA1Managed();
#pragma warning restore CA5350
            var encoded = sha1.ComputeHash(Encoding.UTF8.GetBytes(value));
            return Convert.ToBase64String(encoded);
        }
    }
}