using System;
using System.Text;
using Microsoft.AspNetCore.DataProtection;

namespace Serious.Secrets
{
    /// <summary>
    /// Used to protect and unprotect a secret using the <see cref="IDataProtector"/> API.
    /// </summary>
    public interface ISecretProtector
    {
        /// <summary>
        /// Protects a secret value and returns the Base64 encoded version of that secret.
        /// </summary>
        /// <param name="secret">The clear text secret to protect</param>
        /// <returns>Base64 encoded string of the protected secret.</returns>
        string Protect(string secret);
        
        /// <summary>
        /// Unprotects a Base64 encoded secret and returns the clear text secret.
        /// </summary>
        /// <param name="encrypted">Base64 encoded secret to decode and unprotect</param>
        /// <returns>The clear text secret.</returns>
        string? Unprotect(string encrypted);
    }
}