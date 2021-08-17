using System.Text;
using Microsoft.AspNetCore.DataProtection;

namespace Serious.Abbot.CommandLine.Services
{
    public class TokenProtector
    {
        readonly IDataProtector _protector;

        public TokenProtector() : this(DataProtectionProvider.Create("abbot"))
        {
        }

        public TokenProtector(IDataProtectionProvider provider) : this(provider.CreateProtector("Store Abbot API Token"))
        {
        }

        public TokenProtector(IDataProtector protector)
        {
            _protector = protector;
        }
        
        public byte[] Protect(string token)
        {
            var plainTextToken = Encoding.UTF8.GetBytes(token);
            return _protector.Protect(plainTextToken);
        }
        
        public string? Unprotect(byte[] encrypted)
        {
            if (encrypted.Length == 0)
            {
                return null;
            }
            var unencrypted = _protector.Unprotect(encrypted);
            if (unencrypted.Length == 0)
            {
                return null;
            }
            
            // TODO: Handle or propagate CryptographicException?
            return Encoding.UTF8.GetString(unencrypted);
        }
    }
}