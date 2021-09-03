using System;
using System.Text;
using System.Threading.Tasks;
using Serious.IO;
using Serious.Secrets;
using TestFakes;
using Xunit;

public class SecretStoreTests
{
    public class TheLoadAsyncMethod
    {
        [Fact]
        public async Task LoadsSecretFromTheUnderlyingStore()
        {
            var fileSystem = new FakeFileSystem();
            var lazySecretsFile = new Lazy<IFileInfo>(() => fileSystem.GetFile(".abbot/secrets/SECRET-GUID"));
            var secretProtector = new FakeSecretProtector();
            var secretStore = new SecretStore(lazySecretsFile, secretProtector);
            var secretsFile = fileSystem.GetFile($".abbot/secrets/SECRET-GUID");
            var key1 = Convert.ToBase64String(Encoding.UTF8.GetBytes("Key1"));
            var secret1 = secretProtector.Protect("The Cake is a Lie");
            var key2 = Convert.ToBase64String(Encoding.UTF8.GetBytes("Key2"));
            var secret2 = secretProtector.Protect("Soylent Green is people!");
            await secretsFile.WriteAllTextAsync($"{key1}|{secret1}\n{key2}|{secret2}\n");

            await secretStore.LoadAsync();
            
            Assert.Equal("The Cake is a Lie", secretStore["Key1"]);
            Assert.Equal("Soylent Green is people!", secretStore["Key2"]);
        }
    }

    public class TheIndexerMethod
    {
        [Fact]
        public async Task ReturnsNullWhenNotFound()
        {
            var fileSystem = new FakeFileSystem();
            var lazySecretsFile = new Lazy<IFileInfo>(() => fileSystem.GetFile(".abbot/secrets/SECRET-GUID"));
            var secretProtector = new FakeSecretProtector();
            var secretStore = new SecretStore(lazySecretsFile, secretProtector);
            await secretStore.LoadAsync();

            var secret = secretStore["Key1"];
            
            var secretsFile = fileSystem.GetFile($"~/.abbot/secrets/SECRET-GUID");
            Assert.False(secretsFile.Exists);
            Assert.Null(secret);
        }
    }

    public class TheSaveAsyncMethod
    {
        [Fact]
        public async Task SavesSetSecretsToFile()
        {
            var fileSystem = new FakeFileSystem();
            var lazySecretsFile = new Lazy<IFileInfo>(() => fileSystem.GetFile(".abbot/secrets/SECRET-GUID"));
            var secretProtector = new FakeSecretProtector();
            var secretStore = new SecretStore(lazySecretsFile, secretProtector);
            await secretStore.LoadAsync();
            secretStore.SetSecret("Key1", "The Cake is a Lie");
            secretStore.SetSecret("Key2", "Soylent Green is people!");
            Assert.Equal("The Cake is a Lie", secretStore["Key1"]);
            Assert.Equal("Soylent Green is people!", secretStore["Key2"]);

            await secretStore.SaveAsync();
            
            var secretsFile = fileSystem.GetFile($".abbot/secrets/SECRET-GUID");
            Assert.True(secretsFile.Exists);
            var key1 = Convert.ToBase64String(Encoding.UTF8.GetBytes("Key1"));
            var secret1 = secretProtector.Protect("The Cake is a Lie");
            var key2 = Convert.ToBase64String(Encoding.UTF8.GetBytes("Key2"));
            var secret2 = secretProtector.Protect("Soylent Green is people!");
            var contents = await secretsFile.ReadAllTextAsync();
            Assert.Equal($"{key1}|{secret1}\n{key2}|{secret2}\n", contents);
        }
    }
}
