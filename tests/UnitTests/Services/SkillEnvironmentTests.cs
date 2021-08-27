using System.Threading.Tasks;
using Serious.Abbot.CommandLine.Services;
using Serious.Abbot.Entities;
using Serious.Abbot.Messages;
using UnitTests.Fakes;
using Xunit;

public class SkillEnvironmentTests
{
    public class TheConstructor
    {
        [Fact]
        public void CreatesEnvironmentInstanceButNothingOnDisk()
        {
            var skillDirectory = new FakeDirectoryInfo("./skills/my-skill");
            
            var environment = new SkillEnvironment(skillDirectory);
            
            Assert.False(environment.Exists);
            Assert.False(skillDirectory.Exists);
        }
    }

    public class TheCreateAsyncMethod
    {
        [Fact]
        public async Task CreatesSkillDirectoryAndFiles()
        {
            var skillDirectory = new FakeDirectoryInfo("./skills/my-skill");
            var environment = new SkillEnvironment(skillDirectory);
            var response = new SkillGetResponse
            {
                CodeHash = "abc123",
                Language = CodeLanguage.Python
            };

            await environment.CreateAsync(response);
            
            Assert.True(environment.Exists);
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
            var environment = new SkillEnvironment(skillDirectory);
            var response = new SkillGetResponse
            {
                CodeHash = "abc123",
                Language = CodeLanguage.Python
            };

            await environment.CreateAsync(response);
            
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
            var environment = new SkillEnvironment(skillDirectory);
            var response = new SkillGetResponse
            {
                CodeHash = "abc123",
                Language = language,
                Code = "// This is the code"
            };

            await environment.CreateAsync(response);
            
            var codeFile = skillDirectory.GetFile($"main.{ext}");
            Assert.True(codeFile.Exists);
            var written = await codeFile.ReadAllTextAsync();
            Assert.Equal("// This is the code", written);
        }
        
        [Fact]
        public async Task WritesCSharpCodeFileToDiskWithDirective()
        {
            var skillDirectory = new FakeDirectoryInfo("./skills/my-skill");
            var environment = new SkillEnvironment(skillDirectory);
            var response = new SkillGetResponse
            {
                CodeHash = "abc123",
                Language = CodeLanguage.CSharp,
                Code = "// This is the code"
            };

            await environment.CreateAsync(response);
            
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
            var environment = new DevelopmentEnvironment(directory);
            await environment.EnsureAsync();
            var skillEnvironment = new SkillEnvironment(directory.GetSubdirectory("my-skill"));
            var response = new SkillGetResponse
            {
                CodeHash = "abc123",
                Language = language,
                Code = "# This is the code"
            };
            await skillEnvironment.CreateAsync(response);

            var codeResult = await skillEnvironment.GetCodeAsync(environment);
            
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
            var environment = new DevelopmentEnvironment(directory);
            await environment.EnsureAsync();
            var skillEnvironment = new SkillEnvironment(directory.GetSubdirectory("my-skill"));
            var response = new SkillGetResponse
            {
                CodeHash = "abc123",
                Language = language,
                Code = "#load \".meta/globals.csx\" // This is required for Intellisense in VS Code, etc. DO NOT TOUCH THIS LINE!\n# This is the code"
            };
            await skillEnvironment.CreateAsync(response);

            var codeResult = await skillEnvironment.GetCodeAsync(environment);
            
            Assert.True(codeResult.IsSuccess);
            Assert.Equal("# This is the code", codeResult.Code);
        }

        [Fact]
        public async Task ReportsErrorWhenEnvironmentDoesNotExist()
        {
            var directory = new FakeDirectoryInfo("./skills");
            var environment = new DevelopmentEnvironment(directory);
            var skillEnvironment = new SkillEnvironment(directory.GetSubdirectory("my-skill"));
            
            var codeResult = await skillEnvironment.GetCodeAsync(environment);
            
            Assert.False(codeResult.IsSuccess);
            Assert.Null(codeResult.Code);
            Assert.Equal("The specified directory is not an Abbot Skills folder. Either specify the directory where you've initialized an environment, or initialize a new one using `abbot init`", codeResult.ErrorMessage);
        }
        
        [Fact]
        public async Task ReportsErrorWhenSkillDirectoryDoesNotExist()
        {
            var directory = new FakeDirectoryInfo("./skills");
            var environment = new DevelopmentEnvironment(directory);
            await environment.EnsureAsync();
            var skillEnvironment = new SkillEnvironment(directory.GetSubdirectory("my-skill"));
            
            var codeResult = await skillEnvironment.GetCodeAsync(environment);
            
            Assert.False(codeResult.IsSuccess);
            Assert.Null(codeResult.Code);
            Assert.Equal("The directory ./skills/my-skill does not exist. Have you run `abbot download my-skill` yet?", codeResult.ErrorMessage);
        }
        
        [Fact]
        public async Task ReportsErrorWhenCodeDoesNotExist()
        {
            var directory = new FakeDirectoryInfo("./skills");
            var environment = new DevelopmentEnvironment(directory);
            await environment.EnsureAsync();
            var skillDirectory = directory.GetSubdirectory("my-skill");
            var skillEnvironment = new SkillEnvironment(skillDirectory);
            skillDirectory.Create();
            
            var codeResult = await skillEnvironment.GetCodeAsync(environment);
            
            Assert.False(codeResult.IsSuccess);
            Assert.Null(codeResult.Code);
            Assert.Equal("Did not find a code file in ./skills/my-skill", codeResult.ErrorMessage);
        }
    }
}
