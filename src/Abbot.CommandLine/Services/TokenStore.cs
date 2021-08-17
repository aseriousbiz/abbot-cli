using System.IO;
using System.Threading.Tasks;

namespace Serious.Abbot.CommandLine.Services
{
    public class TokenStore
    {
        readonly TokenProtector _protector;
        readonly FileInfo _tokenFile;

        public TokenStore(TokenProtector protector, FileInfo tokenFile)
        {
            _protector = protector;
            _tokenFile = tokenFile;
        }

        public bool Empty => !_tokenFile.Exists;

        public async Task StoreTokenAsync(string token)
        {
            var encrypted = _protector.Protect(token);
            await using var writer = _tokenFile.Create();;
            await writer.WriteAsync(encrypted);
        }
        
        public async Task<string?> RetrieveTokenAsync()
        {
            if (!_tokenFile.Exists)
            {
                return null;
            }
            var encrypted = await File.ReadAllBytesAsync(_tokenFile.FullName);
            if (encrypted.Length == 0)
            {
                return null;
            }
            return _protector.Unprotect(encrypted);
        }
    }
}