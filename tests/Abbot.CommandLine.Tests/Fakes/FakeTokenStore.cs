using System;
using Serious.IO.CommandLine.Services;
using Serious.Secrets;

namespace UnitTests.Fakes
{
    public class FakeTokenStore : TokenStore
    {
        public FakeTokenStore(FakeFileInfo secretIdFile, Func<string, ISecretStore> secretStoreConstructor)
            : base(secretIdFile, secretStoreConstructor)
        {
            SecretIdFile = secretIdFile;
        }
        
        public FakeFileInfo SecretIdFile { get; }
    }
}