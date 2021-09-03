using System;
using Serious.IO;
using Serious.Secrets;

namespace UnitTests.Fakes
{
    public class FakeSecretStore : SecretStore
    {
        public FakeSecretStore(
            string secretsFilePath,
            IFileSystem fileSystem,
            ISecretProtector protector) : base(new Lazy<IFileInfo>(() => fileSystem.GetFile(secretsFilePath)), protector)
        {
        }
    }
}