using System;
using System.Threading.Tasks;
using Serious.IO.CommandLine.Commands;

namespace Serious.IO.CommandLine.Editors
{
    /// <summary>
    /// Methods for writing files to help Omnisharp out.
    /// </summary>
    public static class OmniSharpHelpers
    {
        /// <summary>
        /// Writes an Omnisharp config file. This helps editors such as VS Code that use Omnisharp to provide a
        /// C# language server to the editor.
        /// </summary>
        /// <param name="directory">The skill directory.</param>
        /// <param name="relativeDirectoryPath">Relative path to the directory containing the reference.rsp file</param>
        public static Task<IFileInfo> WriteConfigFileAsync(IDirectoryInfo directory, string relativeDirectoryPath)
        {
            var referencePath = relativeDirectoryPath + ".abbot/references.rsp";
            string configJson = @"{
    ""script"": {
            ""enabled"": true,
            ""defaultTargetFramework"": ""net5.0"",
            ""enableScriptNuGetReferences"": true,
            ""RspFilePath"": """ + referencePath + @"""
        }
    }";
            return WriteFileAsync(directory, "omnisharp.json", configJson);
        }

        /// <summary>
        /// Writes a Roslyn RSP file into the directory.
        /// </summary>
        /// <param name="directory">The skill directory.</param>
        public static Task<IFileInfo> WriteRspFileAsync(IDirectoryInfo directory)
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
            return WriteFileAsync(directory, "references.rsp", rsp);
        }

        /// <summary>
        /// Write a globals.csx that's loaded by the main script. This allows us to specify NuGet package references.
        /// </summary>
        /// <param name="directory">The skill directory.</param>
        public static Task<IFileInfo> WriteGlobalsCsxFileAsync(IDirectoryInfo directory)
        {
            // TODO: We need to get this list from Abbot and not hard-code it.
            const string globals = @"#r ""nuget:NodaTime,3.0.5""
#r ""nuget:HtmlAgilityPack,1.11.34""
#r ""nuget:Abbot.Scripting.Stubs,0.9.0""

var Bot = new Serious.Abbot.Scripting.Bot();
";
            return WriteFileAsync(directory, "globals.csx", globals);
        }

        static async Task<IFileInfo> WriteFileAsync(IDirectoryInfo directory, string filename, string contents)
        {
            var file = directory.GetFile(filename);
            await file.WriteAllTextAsync(contents);
            return file;
        }

        const string LoadDirective = $"#load \"{GetCommand.SkillMetaFolder}/globals.csx\" // This is required for Intellisense in VS Code, etc. DO NOT TOUCH THIS LINE!";

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
                : $"{LoadDirective}\n{code}";
        }
        
        /// <summary>
        /// Removes the OmniSharp directive to load the globals.csx file exists at the beginning of the file. If it
        /// does not, this prepends it. The directive enables Intellisense when working with VS Code.
        /// </summary>
        /// <param name="code">The C# code</param>
        /// <returns></returns>
        public static string RemoveGlobalsDirective(string code)
        {
            var stripped = !code.StartsWith(LoadDirective, StringComparison.Ordinal)
                ? code
                : code[LoadDirective.Length..];

            if (stripped.StartsWith("\r\n", StringComparison.Ordinal))
            {
                stripped = stripped[2..];
            }
            else if (stripped[0] == '\n')
            {
                stripped = stripped[1..];
            }

            return stripped;
        }
    }
}