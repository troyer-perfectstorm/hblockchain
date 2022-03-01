using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBlockchain
{
    public record struct Hash512
    {
        public const int Length = 64;

        public byte[] Bytes { get; }

        public Hash512(byte[] bytes)
        {
            if (bytes == null || bytes.Length != Length)
                throw new ArgumentException($"{nameof(Hash512)} must be {Length} bytes long");

            Bytes = new byte[Length];
            Array.Copy(bytes, Bytes, Length);
        }

        public Hash512(byte[] bytes1, byte[] bytes2)
        {
            if (bytes1 == null)
                throw new ArgumentNullException(nameof(bytes1));
            if (bytes2 == null)
                throw new ArgumentNullException(nameof(bytes2));
            if (bytes1.Length + bytes2.Length != Length)
                throw new ArgumentException($"{nameof(Hash512)} must be {Length} bytes long");

            Bytes = new byte[Length];
            Buffer.BlockCopy(bytes1, 0, Bytes, 0, bytes1.Length);
            Buffer.BlockCopy(bytes2, 0, Bytes, bytes1.Length, bytes2.Length);
        }

        public (byte[] first, byte[] second) Split()
        {
            const int half = Length / 2;
            var first = new byte[half];
            var second = new byte[half];

            Buffer.BlockCopy(Bytes, 0, first, 0, half);
            Buffer.BlockCopy(Bytes, half, second, 0, half);

            return (first, second);
        }

        public override string ToString()
        {
            return Convert.ToBase64String(Bytes);
        }
    }
}
