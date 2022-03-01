using System.Security.Cryptography;

namespace HBlockchain
{
    public class ECSigner : ISigner
    {
        private ECDsa ECDSA { get; }
        public Hash512 PublicKey { get; }

        public ECSigner(Hash256 privateKey, Hash512 publicKey)
        {
            PublicKey = publicKey;
            var (x, y) = publicKey.Split();
            ECDSA = ECDsa.Create(new ECParameters()
            {
                Curve = ECCurve.NamedCurves.nistP256,
                D = privateKey.Bytes,
                Q = new ECPoint()
                {
                    X = x,
                    Y = y
                }
            });
        }

        public ECSigner(KeyPair keys) : this(keys.PrivateKey, keys.PublicKey) { }

        public Hash512 Sign(ReadOnlySpan<byte> bytes)
        {
            var result = new Hash512(new byte[Hash512.Length]);
            if (!ECDSA.TrySignData(bytes, result.Bytes, HashAlgorithmName.SHA256, out _))
                throw new Exception("Failed to sign data");

            return result;
        }

        public static KeyPair GenerateKeys()
        {
            using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            var export = ecdsa.ExportParameters(true);
            var privateKey = new Hash256(export.D!);
            var publicKey = new Hash512(export.Q.X!, export.Q.Y!);

            return new KeyPair(privateKey, publicKey);
        }
    }

    public record class KeyPair
    {
        public Hash256 PrivateKey { get; }
        public Hash512 PublicKey { get; }

        public KeyPair(Hash256 privateKey, Hash512 publicKey)
        {
            PrivateKey = privateKey;
            PublicKey = publicKey;
        }
    }
}
