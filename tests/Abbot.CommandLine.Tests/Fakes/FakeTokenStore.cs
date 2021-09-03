using System;
using Serious.IO.CommandLine.Services;
using Serious.Secrets;

namespace UnitTests.Fakes
{
    public class FakeTokenStore : TokenStore
    {
        public FakeTokenStore(
            FakeFileInfo secretsIdFile,
            FakeFileInfo secretsDirectoryFile,
            Func<string, string?, ISecretStore> secretStoreConstructor)
            : base(secretsIdFile, secretsDirectoryFile, secretStoreConstructor)
        {
            SecretsIdFile = secretsIdFile;
        }
        
        public FakeFileInfo SecretsIdFile { get; }
    }
}