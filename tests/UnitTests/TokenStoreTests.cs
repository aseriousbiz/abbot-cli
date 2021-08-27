using System;
using System.Text;
using System.Threading.Tasks;
using Serious.Abbot.CommandLine.Services;
using UnitTests.Fakes;
using Xunit;

public class TokenStoreTests
{
    public class TheStoreTokenAsyncMethod
    {
        [Fact]
        public async Task StoresTokenInAFile()
        {
            var tokenProtector = new FakeTokenProtector();
            var tokenFile = new FakeFileInfo("./TOKEN");
            var tokenStore = new TokenStore(tokenProtector, tokenFile);

            await tokenStore.StoreTokenAsync("SOME TOKEN");
            
            Assert.True(tokenFile.Exists);
            var stored = await tokenFile.ReadAllBytesAsync();
            // The FakeTokenProtector just uses Base64 encoding.
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(Encoding.UTF8.GetString(stored)));
            Assert.Equal("SOME TOKEN", decoded);
            var retrieved = await tokenStore.RetrieveTokenAsync();
            Assert.Equal("SOME TOKEN", retrieved);
        }
    }

    public class TheRetrieveTokenAsyncMethod
    {
        [Fact]
        public async Task ReturnsNullWhenFileDoesNotExist()
        {
            var tokenProtector = new FakeTokenProtector();
            var tokenFile = new FakeFileInfo("./some-file");
            var tokenStore = new TokenStore(tokenProtector, tokenFile);

            var result = await tokenStore.RetrieveTokenAsync();
            
            Assert.False(tokenFile.Exists);
            Assert.Null(result);
        }
    }
}
