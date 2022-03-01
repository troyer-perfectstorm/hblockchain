using System.Security.Cryptography;
using System.Text;

namespace HBlockchain
{
    public record Transaction
    {
        public Hash256 Hash { get; set; }
        public Hash512 Signature { get; set; }

        public DateTime Timestamp { get; set; }
        public Hash256 From { get; set; }
        public int Amount { get; set; }
        public Hash256 To { get; set; }

        public static Transaction Create(Hash256 from, int amount, Hash256 to, ISigner signer)
        {
            var now = DateTime.UtcNow;
            var input = new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(now.ToString() + from.ToString() + amount.ToString() + to.ToString()));

            var hashBytes = SHA256.HashData(input);
            var signature = signer.Sign(input);

            return new Transaction()
            {
                Hash = new Hash256(hashBytes),
                Signature = signature,

                Timestamp = now,
                From = new Hash256(from.Bytes),
                Amount = amount,
                To = new Hash256(to.Bytes)
            };
        }

        public bool IsValid(Hash512 publicKey, IVerifier verifier)
        {
            var input = new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(Timestamp.ToString() + From.ToString() + Amount.ToString() + To.ToString()));
            return verifier.IsValidSignature(publicKey, input, Signature);
        }

        public Transaction Duplicate()
        {
            return new Transaction(this);
        }
    }
}
