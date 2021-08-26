using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Serious.Abbot.CommandLine.Editors;
using Serious.Abbot.CommandLine.Services;
using Serious.Abbot.Entities;
using Serious.Abbot.Messages;

namespace Serious.Abbot.CommandLine.Commands
{
    public class GetCommand : Command
    {
        public GetCommand() : base("get", "Downloads the specified skill code into a directory named after the skill.")
        {
            Add(new Argument<string>("skill", () => string.Empty, "The name of the skill"));
            var directoryOption = new Option<string>("--directory", "The Abbot Skills folder. If omitted, assumes the current directory.");
            directoryOption.AddAlias("-d");
            AddOption(directoryOption);
            var forceOption = new Option<bool>("--force", "If true, overwrites the local skill code if it exists even if it has changes.");
            forceOption.AddAlias("-f");
            AddOption(forceOption);
            Handler = CommandHandler.Create<string, string, bool>(HandleDownloadCommandAsync);
        }

        static async Task<int> HandleDownloadCommandAsync(string skill, string directory, bool force)
        {
            var environment = DevelopmentEnvironment.GetEnvironment(directory);
            if (!environment.IsInitialized)
            {
                var directoryType = directory == "." ? "current" : "specified";
                Console.WriteLine($"The {directoryType} directory is not an Abbot Skills folder. Either specify the directory where you've initialized an environment, or initialize a new one using `abbot init`");
                return 1;
            }
            
            var skillInfo = await GetSkillInfoAsync(skill, environment);
            if (skillInfo is null)
            {
                return 1;
            }      
            
            var extension = skillInfo.Language.GetFileExtension();
            var skillDirectoryPath = Path.Combine(environment.WorkingDirectory.FullName, skill);
            var codeFilePath = Path.Combine(skillDirectoryPath, $"{skill}.{extension}");
            var concurrencyFilePath = Path.Combine(skillDirectoryPath, ".concurrency");

            bool directoryExists = Directory.Exists(skillDirectoryPath);
            
            if (!force && directoryExists && await HasConcurrencyConflictAsync(codeFilePath, concurrencyFilePath))
            {
                Console.Write("You have local changes to the code that would be overwritten by getting the latest code.\nOverwrite local changes? Hit Y to overwrite, any other key to cancel: ");
                var key = Console.ReadKey();
                if (!(key.KeyChar is 'Y' or 'y'))
                {
                    Console.WriteLine("\nNo local changes were made");
                    return 1;
                }

                Console.WriteLine("\nOverwriting local changes");
            }

            await WriteSkillFilesAsync(skillDirectoryPath, codeFilePath, skillInfo, concurrencyFilePath);

            var verb = directoryExists ? "Updated" : "Created";
            
            Console.WriteLine(@$"{verb} skill directory {skillDirectoryPath}
Edit the code in the directory. When you are ready to deploy it, run 

    abbot deploy {skill}
");
            return 0;
        }

        static async Task WriteSkillFilesAsync(
            string skillDirectoryPath,
            string codeFilePath,
            SkillGetResponse skillInfo,
            string concurrencyFilePath)
        {
            Directory.CreateDirectory(skillDirectoryPath); // noop if directory already exists.
            var concurrencyFile = await FileHelpers.WriteAllTextAsync(concurrencyFilePath, skillInfo.CodeHash);
            concurrencyFile.HideFile();

            var code = skillInfo.Code;
            if (skillInfo.Language is CodeLanguage.CSharp)
            {
                // Write extra files to help VS Code's Intellisense.
                await Omnisharp.WriteConfigFileAsync(skillDirectoryPath);

                var editorMetaDirectory = Path.Combine(skillDirectoryPath, ".editor");
                Directory.CreateDirectory(editorMetaDirectory); // noop if directory already exists.
                await Omnisharp.WriteRspFileAsync(editorMetaDirectory);
                await Omnisharp.WriteGlobalsCsxFileAsync(editorMetaDirectory);

                code = Omnisharp.EnsureGlobalsDirective(code);
            }
            
            // Write the code file.
            await FileHelpers.WriteAllTextAsync(codeFilePath, code);
        }

        static async Task<SkillGetResponse?> GetSkillInfoAsync(string skill, DevelopmentEnvironment environment)
        {
            var response = await AbbotApi.CreateInstance(environment).GetSkillAsync(skill);
            if (!response.IsSuccessStatusCode)
            {
                await response.HandleUnsuccessfulResponseAsync();
                return null;
            }

            if (response.Content is null)
            {
                await Console.Error.WriteLineAsync("Response content is null");
                return null;
            }

            return response.Content;
        }

        // Returns true if the code was updated on the server since the version stored locally.
        static async Task<bool> HasConcurrencyConflictAsync(string codeFilePath, string concurrencyFilePath)
        {
            if (!File.Exists(codeFilePath))
            {
                return false;
            }
            
            var existingCode = await File.ReadAllTextAsync(codeFilePath);
            existingCode = Omnisharp.RemoveGlobalsDirective(existingCode);
            
            var fi = new FileInfo(concurrencyFilePath);
            // Check concurrency, and don't trust File.Exists, it lies to you
            var existingCodeHash = fi.Exists
                ? await File.ReadAllTextAsync(concurrencyFilePath)
                : string.Empty;

            var codeHash = ComputeSHA1Hash(existingCode);
            return existingCodeHash != codeHash;
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