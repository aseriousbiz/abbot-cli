using System;
using Serious.IO;
using Serious.Secrets;

namespace UnitTests.Fakes
{
    public class FakeSecretStore : SecretStore
    {
        public FakeSecretStore(
            string secretsId,
            IFileSystem fileSystem,
            ISecretProtector protector,
            Func<string, string>? pathHelper = null) : base(secretsId, fileSystem, protector, pathHelper)
        {
        }
    }
}