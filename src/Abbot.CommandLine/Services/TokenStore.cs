using System.Threading.Tasks;
using Serious.Abbot.CommandLine.IO;

namespace Serious.Abbot.CommandLine.Services
{
    public class TokenStore
    {
        readonly TokenProtector _protector;
        readonly IFileInfo _tokenFile;

        public TokenStore(TokenProtector protector, IFileInfo tokenFile)
        {
            _protector = protector;
            _tokenFile = tokenFile;
        }

        public bool Empty => !_tokenFile.Exists;

        public async Task StoreTokenAsync(string token)
        {
            var encrypted = _protector.Protect(token);
            await using var writer = _tokenFile.Create();
            await writer.WriteAsync(encrypted);
        }
        
        public async Task<string?> RetrieveTokenAsync()
        {
            if (!_tokenFile.Exists)
            {
                return null;
            }
            var encrypted = await _tokenFile.ReadAllBytesAsync();
            return encrypted.Length > 0
                ? _protector.Unprotect(encrypted)
                : null;
        }
    }
}