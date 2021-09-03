using System;
using System.Text;
using Serious.Secrets;

namespace TestFakes
{
    public class FakeSecretProtector : ISecretProtector
    {
        public string Protect(string secret)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(secret));
        }

        public string? Unprotect(string encrypted)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(encrypted));
        }
    }
}