using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Serious.Abbot.CommandLine.Editors;
using Serious.Abbot.CommandLine.IO;
using Serious.Abbot.Entities;
using Serious.Abbot.Messages;

namespace Serious.Abbot.CommandLine.Services
{
    public class SkillEnvironment
    {
        readonly string _skill;
        readonly IDirectoryInfo _skillDirectory;
        readonly IDirectoryInfo _skillMetaDirectory;
        readonly IFileInfo _concurrencyFile;

        public SkillEnvironment(string skill, IDirectoryInfo skillDirectory)
        {
            _skill = skill;
            _skillDirectory = skillDirectory;
            _skillMetaDirectory = _skillDirectory.GetSubdirectory(".meta");
            _concurrencyFile = _skillMetaDirectory.GetFile(".concurrency");
        }

        public bool Exists => _skillDirectory.Exists;

        public IDirectoryInfo WorkingDirectory => _skillDirectory;

        /// <summary>
        /// Reads the concurrency file for the skill.
        /// </summary>
        /// <returns></returns>
        public Task<string> ReadConcurrencyFileAsync()
        {
            return _concurrencyFile.Exists
                ? _concurrencyFile.ReadAllTextAsync()
                : Task.FromResult(string.Empty);
        }

        /// <summary>
        /// Writes the specified code hash to the concurrency file.
        /// </summary>
        /// <param name="codeHash">The hash of the code</param>
        public async Task WriteConcurrencyFileAsync(string codeHash)
        {
            await _concurrencyFile.WriteAllTextAsync(codeHash);
            _concurrencyFile.Hide();
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
            _skillDirectory.Create(); // noop if directory already exists.
            _skillMetaDirectory.Create(); // noop if directory already exists.

            await WriteConcurrencyFileAsync(skillInfo.CodeHash);
            await WriteCodeAsync(skillInfo.Code, skillInfo.Language);
            
            if (skillInfo.Language is CodeLanguage.CSharp)
            {
                await WriteGlobalsCsxFileAsync();
            }
        }

        public Task WriteCodeAsync(string code, CodeLanguage language)
        {
            var codeFile = GetCodeFile(language);
            var contents = language is CodeLanguage.CSharp
                ? Omnisharp.EnsureGlobalsDirective(code)
                : code;
            return codeFile.WriteAllTextAsync(contents);
        }
        
        /// <summary>
        /// Returns true if the local code for the skill has local changes.
        /// </summary>
        /// <param name="language">The language for the skill</param>
        public async Task<bool> HasLocalChangesAsync(CodeLanguage language)
        {
            var codeFile = GetCodeFile(language);
            if (!codeFile.Exists)
            {
                return false;
            }
            
            var existingCode = await codeFile.ReadAllTextAsync();
            existingCode = Omnisharp.RemoveGlobalsDirective(existingCode);

            var existingCodeHash = await ReadConcurrencyFileAsync();

            var codeHash = ComputeSHA1Hash(existingCode);
            return existingCodeHash != codeHash;
        }

        /// <summary>
        /// Retrieve the path to the code file when we don't know the language.
        /// </summary>
        public IFileInfo? GetCodeFile()
        {
           return  Enum.GetValues<CodeLanguage>()
                .Select(GetCodeFile)
                .SingleOrDefault(f => f.Exists);
        }

        IFileInfo GetCodeFile(CodeLanguage language)
        {
            var extension = language.GetFileExtension();
            return _skillDirectory.GetFile($"{_skill}.{extension}");
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