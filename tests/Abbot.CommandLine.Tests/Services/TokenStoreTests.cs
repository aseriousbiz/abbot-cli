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
            var secretIdFile = fileSystem.GetFile("./foo/SecretsId");
            await secretIdFile.WriteAllTextAsync("SECRET_ID");
            var tokenStore = new TokenStore(secretIdFile, FakeConstructors.SecretStoreConstructor(fileSystem, new FakeSecretProtector()));

            await tokenStore.StoreTokenAsync("SOME TOKEN");
            
            Assert.True(secretIdFile.Exists);
            var stored = await tokenStore.RetrieveTokenAsync();
            Assert.Equal("SOME TOKEN", stored);
            Assert.Null(fileSystem.GetEnvironmentVariable("Abbot:SECRET_ID:AbbotApiToken"));
        }
    }

    public class TheRetrieveTokenAsyncMethod
    {
        [Fact]
        public async Task CreatesSecretIdFileAndReturnsNullWhenFileDoesNotExist()
        {
            var fileSystem = new FakeFileSystem();
            var secretIdFile = fileSystem.GetFile("./some-file");
            var tokenStore = new TokenStore(secretIdFile, FakeConstructors.SecretStoreConstructor(fileSystem, new FakeSecretProtector()));
            Assert.False(secretIdFile.Exists);

            var result = await tokenStore.RetrieveTokenAsync();
            
            Assert.True(secretIdFile.Exists);
            Assert.Null(result);
        }
        
        [Fact]
        public async Task RetrievesTokenInSecretStorePointedToBySecretIdFile()
        {
            var fileSystem = new FakeFileSystem();
            var secretIdFile = fileSystem.GetFile("./some-file");
            await secretIdFile.WriteAllTextAsync("SECRET-IDENTIFIER");
            var secretStoreConstructor = FakeConstructors.SecretStoreConstructor(fileSystem, new FakeSecretProtector());
            var secretStore = secretStoreConstructor("SECRET-IDENTIFIER");
            secretStore.SetSecret("AbbotApiToken", "SUPER SECRET");
            await secretStore.SaveAsync();
            var tokenStore = new TokenStore(secretIdFile, secretStoreConstructor);
            
            var result = await tokenStore.RetrieveTokenAsync();
            
            Assert.Equal("SUPER SECRET", result);
        }
    }
}
