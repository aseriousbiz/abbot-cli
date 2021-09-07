using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.DataProtection;

namespace UnitTests.Fakes
{
    public class FakeDataProtector : IDataProtector
    {
        readonly Dictionary<byte[], byte[]> _protected = new(new ArrayEqualityComparer<byte>());
        readonly Dictionary<byte[], byte[]> _unprotected = new(new ArrayEqualityComparer<byte>());
        
        public IDataProtector CreateProtector(string purpose)
        {
            return new FakeDataProtector();
        }

        public void Setup(string plainText, string protectedText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            var protectedBytes = Encoding.UTF8.GetBytes(protectedText);

            Setup(plainTextBytes, protectedBytes);
        }

        void Setup(byte[] plainText, byte[] result)
        {
            _protected.Add(plainText, result);
            _unprotected.Add(result, plainText);
        }

        public byte[] Protect(byte[] plaintext)
        {
            return _protected[plaintext];
        }

        public byte[] Unprotect(byte[] protectedData)
        {
            return _unprotected[protectedData];
        }
    }

    public sealed class ArrayEqualityComparer<T> : IEqualityComparer<T[]>
    {
        // You could make this a per-instance field with a constructor parameter
        static readonly EqualityComparer<T> ElementComparer
            = EqualityComparer<T>.Default;

        public bool Equals(T[]? x, T[]? y)
        {
            if (x == y)
            {
                return true;
            }
            if (x is null || y is null)
            {
                return false;
            }
            
            return x.Length == y.Length && x.SequenceEqual(y);
        }

        public int GetHashCode(T[]? obj)
        {
            unchecked
            {
                return obj is null
                    ? 0
                    : obj.Aggregate(17,
                        (current, element) => current * 31 + (element is null ? 1 : ElementComparer.GetHashCode(element)));
            }
        }
    }
}