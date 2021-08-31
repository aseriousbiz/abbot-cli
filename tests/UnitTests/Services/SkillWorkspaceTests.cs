using System.Threading.Tasks;
using Serious.Abbot.CommandLine.Services;
using Serious.Abbot.Entities;
using Serious.Abbot.Messages;
using UnitTests.Fakes;
using Xunit;

public class SkillWorkspaceTests
{
    public class TheConstructor
    {
        [Fact]
        public void CreatesSkillWorkspaceInstanceButNothingOnDisk()
        {
            var skillDirectory = new FakeDirectoryInfo("./skills/my-skill");
            
            var workspace = new SkillWorkspace(skillDirectory);
            
            Assert.False(workspace.Exists);
            Assert.False(skillDirectory.Exists);
        }
    }

    public class TheCreateAsyncMethod
    {
        [Fact]
        public async Task CreatesSkillDirectoryAndFiles()
        {
            var skillDirectory = new FakeDirectoryInfo("./skills/my-skill");
            var workspace = new SkillWorkspace(skillDirectory);
            var response = new SkillGetResponse
            {
                CodeHash = "abc123",
                Language = CodeLanguage.Python
            };

            await workspace.CreateAsync(response);
            
            Assert.True(workspace.Exists);
            Assert.True(skillDirectory.Exists);
            var skillMetaDirectory = skillDirectory.GetSubdirectory(".meta");
            Assert.True(skillMetaDirectory.Exists);
            var concurrencyFile = skillMetaDirectory.GetFile(".concurrency");
            Assert.True(concurrencyFile.Exists);
        }
        
        [Fact]
        public async Task WritesConcurrencyFileToDisk()
        {
            var skillDirectory = new FakeDirectoryInfo("./skills/my-skill");
            var workspace = new SkillWorkspace(skillDirectory);
            var response = new SkillGetResponse
            {
                CodeHash = "abc123",
                Language = CodeLanguage.Python
            };

            await workspace.CreateAsync(response);
            
            var concurrencyFile = skillDirectory.GetSubdirectory(".meta").GetFile(".concurrency");
            Assert.True(concurrencyFile.Exists);
            var written = await concurrencyFile.ReadAllTextAsync();
            Assert.Equal("abc123", written);
        }
        
        [Theory]
        [InlineData(CodeLanguage.Python, "py")]
        [InlineData(CodeLanguage.JavaScript, "js")]
        public async Task WritesCodeFileToDisk(CodeLanguage language, string ext)
        {
            var skillDirectory = new FakeDirectoryInfo("./skills/my-skill");
            var workspace = new SkillWorkspace(skillDirectory);
            var response = new SkillGetResponse
            {
                CodeHash = "abc123",
                Language = language,
                Code = "// This is the code"
            };

            await workspace.CreateAsync(response);
            
            var codeFile = skillDirectory.GetFile($"main.{ext}");
            Assert.True(codeFile.Exists);
            var written = await codeFile.ReadAllTextAsync();
            Assert.Equal("// This is the code", written);
        }
        
        [Fact]
        public async Task WritesCSharpCodeFileToDiskWithDirective()
        {
            var skillDirectory = new FakeDirectoryInfo("./skills/my-skill");
            var workspace = new SkillWorkspace(skillDirectory);
            var response = new SkillGetResponse
            {
                CodeHash = "abc123",
                Language = CodeLanguage.CSharp,
                Code = "// This is the code"
            };

            await workspace.CreateAsync(response);
            
            var codeFile = skillDirectory.GetFile("main.csx");
            Assert.True(codeFile.Exists);
            var written = await codeFile.ReadAllTextAsync();
            Assert.Equal("#load \".meta/globals.csx\" // This is required for Intellisense in VS Code, etc. DO NOT TOUCH THIS LINE!\n// This is the code", written);
        }
    }

    public class TheGetCodeAsyncMethod
    {
        [Theory]
        [InlineData(CodeLanguage.CSharp)]
        [InlineData(CodeLanguage.Python)]
        [InlineData(CodeLanguage.JavaScript)]
        public async Task RetrievesTheLocalCodeForEachLanguage(CodeLanguage language)
        {
            var directory = new FakeDirectoryInfo("./skills");
            var workspace = new Workspace(directory, true);
            await workspace.EnsureAsync();
            var skillWorkspace = new SkillWorkspace(directory.GetSubdirectory("my-skill"));
            var response = new SkillGetResponse
            {
                CodeHash = "abc123",
                Language = language,
                Code = "# This is the code"
            };
            await skillWorkspace.CreateAsync(response);

            var codeResult = await skillWorkspace.GetCodeAsync(workspace);
            
            Assert.True(codeResult.IsSuccess);
            Assert.Equal("# This is the code", codeResult.Code);
        }
        
        [Theory]
        [InlineData(CodeLanguage.CSharp)]
        [InlineData(CodeLanguage.Python)]
        [InlineData(CodeLanguage.JavaScript)]
        public async Task StripsLoadDirective(CodeLanguage language)
        {
            var directory = new FakeDirectoryInfo("./skills");
            var workspace = new Workspace(directory, true);
            await workspace.EnsureAsync();
            var skillWorkspace = new SkillWorkspace(directory.GetSubdirectory("my-skill"));
            var response = new SkillGetResponse
            {
                CodeHash = "abc123",
                Language = language,
                Code = "#load \".meta/globals.csx\" // This is required for Intellisense in VS Code, etc. DO NOT TOUCH THIS LINE!\n# This is the code"
            };
            await skillWorkspace.CreateAsync(response);

            var codeResult = await skillWorkspace.GetCodeAsync(workspace);
            
            Assert.True(codeResult.IsSuccess);
            Assert.Equal("# This is the code", codeResult.Code);
        }

        [Theory]
        [InlineData(true, "specified")]
        [InlineData(false, "current")]
        public async Task ReportsErrorWhenWorkspaceDirectoryDoesNotExist(bool directorySpecified, string expectedType)
        {
            var directory = new FakeDirectoryInfo("./skills");
            var workspace = new Workspace(directory, directorySpecified);
            var skillWorkspace = new SkillWorkspace(directory.GetSubdirectory("my-skill"));
            
            var codeResult = await skillWorkspace.GetCodeAsync(workspace);
            
            Assert.False(codeResult.IsSuccess);
            Assert.Null(codeResult.Code);
            Assert.Equal($"The {expectedType} directory is not an Abbot Workspace. Either specify the path to an Abbot Workspace, or initialize a new one using `abbot init`", codeResult.ErrorMessage);
        }
        
        [Fact]
        public async Task ReportsErrorWhenSkillDirectoryDoesNotExist()
        {
            var directory = new FakeDirectoryInfo("./skills");
            var workspace = new Workspace(directory, true);
            await workspace.EnsureAsync();
            var skillWorkspace = new SkillWorkspace(directory.GetSubdirectory("my-skill"));
            
            var codeResult = await skillWorkspace.GetCodeAsync(workspace);
            
            Assert.False(codeResult.IsSuccess);
            Assert.Null(codeResult.Code);
            Assert.Equal("The directory ./skills/my-skill does not exist. Have you run `abbot get my-skill` yet?", codeResult.ErrorMessage);
        }
        
        [Fact]
        public async Task ReportsErrorWhenCodeDoesNotExist()
        {
            var directory = new FakeDirectoryInfo("./skills");
            var workspace = new Workspace(directory, true);
            await workspace.EnsureAsync();
            var skillDirectory = directory.GetSubdirectory("my-skill");
            var skillWorkspace = new SkillWorkspace(skillDirectory);
            skillDirectory.Create();
            
            var codeResult = await skillWorkspace.GetCodeAsync(workspace);
            
            Assert.False(codeResult.IsSuccess);
            Assert.Null(codeResult.Code);
            Assert.Equal("Did not find a code file in ./skills/my-skill", codeResult.ErrorMessage);
        }
    }
}
