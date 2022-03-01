using System.Security.Cryptography;

namespace HBlockchain
{
    public record Account
    {
        public Hash256 Id { get; }
        public Hash512 PublicKey { get; }

        public Account(Hash256 id, Hash512 publicKey)
        {
            Id = id;
            PublicKey = publicKey;
        }

        public static Account Create(Hash512 publicKey)
        {
            var newId = new Hash256(SHA256.HashData(publicKey.Bytes));
            return new Account(newId, publicKey);
        }
    }
}
