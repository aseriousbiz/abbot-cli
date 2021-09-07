using System.Threading.Tasks;
using Serious.IO.CommandLine.Services;
using TestFakes;
using UnitTests.Fakes;
using Xunit;

public class TokenStoreTests
{
    public class TheStoreTokenAsyncMethod
    {
        [Fact]
        public async Task StoresTokenInSecretStore()
        {
            var fileSystem = new FakeFileSystem();
            var secretsIdFile = fileSystem.GetFile("./some-file");
            await secretsIdFile.WriteAllTextAsync("secrets-identifier");
            var secretsDirectoryFile = fileSystem.GetFile("./secrets-directory");
            var secretStoreConstructor = FakeConstructors.SecretStoreConstructor(fileSystem, new FakeSecretProtector());
            var tokenStore = new TokenStore(secretsIdFile, secretsDirectoryFile, secretStoreConstructor);
            
            await tokenStore.StoreTokenAsync("SOME TOKEN");
            
            Assert.True(secretsIdFile.Exists);
            var stored = await tokenStore.RetrieveTokenAsync();
            Assert.Equal("SOME TOKEN", stored);
            var writtenFile = fileSystem.GetFile(".abbot/secrets/secrets-identifier");
            Assert.True(writtenFile.Exists);
        }
        
        [Fact]
        public async Task GeneratesAndCreatesSecretsIdFile()
        {
            var fileSystem = new FakeFileSystem();
            var secretsIdFile = fileSystem.GetFile("./some-file");
            var secretsDirectoryFile = fileSystem.GetFile("./secrets-directory");
            var secretStoreConstructor = FakeConstructors.SecretStoreConstructor(fileSystem, new FakeSecretProtector());
            var tokenStore = new TokenStore(secretsIdFile, secretsDirectoryFile, secretStoreConstructor);
            Assert.False(secretsIdFile.Exists);
            
            await tokenStore.StoreTokenAsync("SOME TOKEN");
            
            Assert.True(secretsIdFile.Exists);
            var stored = await tokenStore.RetrieveTokenAsync();
            Assert.Equal("SOME TOKEN", stored);
            var writtenFile = fileSystem.GetFile(secretsIdFile.FullName);
            Assert.True(writtenFile.Exists);
        }
        
        [Fact]
        public async Task StoresTokenInAlternateLocation()
        {
            var fileSystem = new FakeFileSystem();
            var secretsIdFile = fileSystem.GetFile("./some-file");
            await secretsIdFile.WriteAllTextAsync("secrets-identifier");
            var secretsDirectoryFile = fileSystem.GetFile("./secrets-directory");
            await secretsDirectoryFile.WriteAllTextAsync("./secrets/alternate");
            var secretStoreConstructor = FakeConstructors.SecretStoreConstructor(fileSystem, new FakeSecretProtector());
            var tokenStore = new TokenStore(secretsIdFile, secretsDirectoryFile, secretStoreConstructor);

            await tokenStore.StoreTokenAsync("SOME TOKEN");
            
            Assert.True(secretsIdFile.Exists);
            var writtenFile = fileSystem.GetFile("./secrets/alternate/secrets-identifier");
            Assert.True(writtenFile.Exists);
            var stored = await tokenStore.RetrieveTokenAsync();
            Assert.Equal("SOME TOKEN", stored);
        }
    }

    public class TheRetrieveTokenAsyncMethod
    {
        [Fact]
        public async Task CreatesSecretsIdFileAndReturnsNullWhenFileDoesNotExist()
        {
            var fileSystem = new FakeFileSystem();
            var secretsIdFile = fileSystem.GetFile("./some-file");
            var secretsDirectoryFile = fileSystem.GetFile("./secrets-directory");
            var secretStoreConstructor = FakeConstructors.SecretStoreConstructor(fileSystem, new FakeSecretProtector());
            var tokenStore = new TokenStore(secretsIdFile, secretsDirectoryFile, secretStoreConstructor);
            Assert.False(secretsIdFile.Exists);

            var result = await tokenStore.RetrieveTokenAsync();
            
            Assert.True(secretsIdFile.Exists);
            Assert.Null(result);
        }
        
        [Fact]
        public async Task RetrievesTokenInSecretStoreAdDefaultLocationForSecretsId()
        {
            var fileSystem = new FakeFileSystem();
            var secretsIdFile = fileSystem.GetFile("./some-file");
            await secretsIdFile.WriteAllTextAsync("SECRET-IDENTIFIER");
            var secretsDirectoryFile = fileSystem.GetFile("./secrets-directory");
            var secretStoreConstructor = FakeConstructors.SecretStoreConstructor(fileSystem, new FakeSecretProtector());
            var secretStore = secretStoreConstructor("SECRET-IDENTIFIER", null);
            secretStore.SetSecret("AbbotApiToken", "SUPER SECRET");
            await secretStore.SaveAsync();
            var tokenStore = new TokenStore(secretsIdFile, secretsDirectoryFile, secretStoreConstructor);
            
            var result = await tokenStore.RetrieveTokenAsync();
            
            Assert.Equal("SUPER SECRET", result);
        }
    }
}
