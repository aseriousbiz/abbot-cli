using Serious.IO;
using Serious.Secrets;

namespace UnitTests.Fakes
{
    public class FakeSecretStore : SecretStore
    {
        public FakeSecretStore(
            string secretsFilePath,
            IFileSystem fileSystem,
            ISecretProtector protector) : base(secretsFilePath, fileSystem, protector)
        {
        }
    }
}