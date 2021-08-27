using System;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Serious.Abbot.CommandLine.Services;

namespace UnitTests.Fakes
{
    public class FakeTokenProtector : TokenProtector
    {
        public FakeTokenProtector() : base(new Base64TokenProtector())
        {
        }

        class Base64TokenProtector : IDataProtector
        {
            public IDataProtector CreateProtector(string purpose)
            {
                return this;
            }

            public byte[] Protect(byte[] plaintext)
            {
                return Encoding.UTF8.GetBytes(Convert.ToBase64String(plaintext));
            }

            public byte[] Unprotect(byte[] protectedData)
            {
                return Convert.FromBase64String(Encoding.UTF8.GetString(protectedData));
            }
        }
    }
}