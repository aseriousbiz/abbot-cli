using System;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Serious.Secrets;
using UnitTests.Fakes;
using Xunit;

public class SecretProtectorTests
{
    public class TheProtectMethod
    {
        [Fact]
        public void ProtectsData()
        {
            var protector = new FakeDataProtector();
            protector.Setup("The cake is a lie", "Everything is fine with the cake");
            var secretProtector = new SecretProtector(protector);

            var encoded = secretProtector.Protect("The cake is a lie");
            
            var expected = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("Everything is fine with the cake"));;
            Assert.Equal(expected, encoded);
        }
    }
    
    public class TheUnprotectMethod
    {
        [Fact]
        public void UnprotectsData()
        {
            var protector = new FakeDataProtector();
            protector.Setup("The cake is a lie", "Everything is fine with the cake");
            var secretProtector = new SecretProtector(protector);
            var protectedData = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("Everything is fine with the cake"));

            var unencoded = secretProtector.Unprotect(protectedData);

            Assert.Equal("The cake is a lie", unencoded);
        }
    }
}