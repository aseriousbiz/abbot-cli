using System;
using System.IO;
using System.Threading.Tasks;
using Serious.Abbot.CommandLine.Commands;

namespace Serious.Abbot.CommandLine.Editors
{
    /// <summary>
    /// Methods for writing files to help Omnisharp out.
    /// </summary>
    public static class Omnisharp
    {
        /// <summary>
        /// Writes an Omnisharp config file. This helps editors such as VS Code that use Omnisharp to provide a
        /// C# language server to the editor.
        /// </summary>
        /// <param name="directory">The skill directory.</param>
        /// <param name="referencePath">Path to the reference.rsp file</param>
        public static Task<FileInfo> WriteConfigFileAsync(string directory, string referencePath)
        {
            string configJson = @"{
    ""script"": {
            ""enabled"": true,
            ""defaultTargetFramework"": ""net5.0"",
            ""enableScriptNuGetReferences"": true,
            ""RspFilePath"": """ + referencePath + @"""
        }
    }";
            var path = Path.Combine(directory, "omnisharp.json");
            return FileHelpers.WriteAllTextAsync(path, configJson);
        }

        /// <summary>
        /// Writes a Roslyn RSP file into the directory.
        /// </summary>
        /// <param name="directory">The skill directory.</param>
        public static Task<FileInfo> WriteRspFileAsync(string directory)
        {
            // TODO: We need to get this list from Abbot and not hard-code it.
            const string rsp = @"/u:System
/u:System.Collections
/u:System.Collections.Concurrent;
/u:System.Collections.Generic
/u:System.Data
/u:System.Dynamic
/u:System.Globalization
/u:System.Linq
/u:System.Linq.Expressions
/u:System.Net.Http
/u:System.Text
/u:System.Text.RegularExpressions
/u:System.Threading
/u:System.Threading.Tasks
/u:Serious.Abbot.Scripting
/u:NodaTime";
            var path = Path.Combine(directory, "references.rsp");
            return FileHelpers.WriteAllTextAsync(path, rsp);
        }

        /// <summary>
        /// Write a globals.csx that's loaded by the main script. This allows us to specify NuGet package references.
        /// </summary>
        /// <param name="directory">The skill directory.</param>
        public static Task<FileInfo> WriteGlobalsCsxFileAsync(string directory)
        {
            // TODO: We need to get this list from Abbot and not hard-code it.
            const string globals = @"#r ""nuget:NodaTime,3.0.5""
#r ""nuget:HtmlAgilityPack,1.11.34""
#r ""nuget:Abbot.Scripting.Stubs,0.9.0""

var Bot = new Serious.Abbot.Scripting.Bot();
";
            var path = Path.Combine(directory, "globals.csx");
            return FileHelpers.WriteAllTextAsync(path, globals);
        }

        const string LoadDirective = $"#load \"{GetCommand.SkillMetaFolder}/globals.csx\" // This is required for Intellisense in VS Code, etc. DO NOT TOUCH THIS LINE!\n";

        /// <summary>
        /// Adds the OmniSharp directive to load the globals.csx file exists at the beginning of the file. If it
        /// does not, this prepends it. The directive enables Intellisense when working with VS Code.
        /// </summary>
        /// <param name="code">The C# code</param>
        /// <returns></returns>
        public static string EnsureGlobalsDirective(string code)
        {
            return code.StartsWith(LoadDirective, StringComparison.Ordinal)
                ? code
                : $"{LoadDirective}{code}";
        }
        
        /// <summary>
        /// Removes the OmniSharp directive to load the globals.csx file exists at the beginning of the file. If it
        /// does not, this prepends it. The directive enables Intellisense when working with VS Code.
        /// </summary>
        /// <param name="code">The C# code</param>
        /// <returns></returns>
        public static string RemoveGlobalsDirective(string code)
        {
            return !code.StartsWith(LoadDirective, StringComparison.Ordinal)
                ? code
                : code[LoadDirective.Length..];
        }
    }
}