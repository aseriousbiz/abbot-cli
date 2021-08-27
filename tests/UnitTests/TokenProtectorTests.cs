using System.Text;
using Serious.Abbot.CommandLine.Services;
using UnitTests.Fakes;
using Xunit;

public class TokenProtectorTests
{
    public class TheProtectMethod
    {
        [Fact]
        public void ProtectsTheUTF8EncodedToken()
        {
            var plainTextBytes = Encoding.UTF8.GetBytes("abcde");
            var protector = new FakeDataProtector();
            var expected = new byte[] { 4, 2, 0, 1 };
            var notExpected = new byte[] { 5, 4, 3, 2 };
            var otherBytes = Encoding.UTF8.GetBytes("abcd");
            protector.AddProtected(plainTextBytes, expected);
            protector.AddProtected(otherBytes, notExpected);
            var tokenProtector = new TokenProtector(protector);
            
            var result = tokenProtector.Protect("abcde");

            Assert.Equal(expected, result);
        }
    }

    public class TheUnprotectMethod
    {
        [Fact]
        public void UnprotectsTheProtectedByteArray()
        {
            var plainTextBytes = Encoding.UTF8.GetBytes("abcde");
            var protector = new FakeDataProtector();
            var expected = new byte[] { 4, 2, 0, 1 };
            var notExpected = new byte[] { 5, 4, 3, 2 };
            var otherBytes = Encoding.UTF8.GetBytes("abcd");
            protector.AddProtected(plainTextBytes, expected);
            protector.AddProtected(otherBytes, notExpected);
            var tokenProtector = new TokenProtector(protector);
            
            var result = tokenProtector.Unprotect(expected);

            Assert.Equal("abcde", result);
        }
    }
}
