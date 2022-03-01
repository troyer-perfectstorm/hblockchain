using System.Security.Cryptography;

namespace HBlockchain
{
    public class ECVerifier : IVerifier
    {
        public bool IsValidSignature(Hash512 publicKey, ReadOnlySpan<byte> input, Hash512 signature)
        {
            var (x, y) = publicKey.Split();
            using var ecdsa = ECDsa.Create(new ECParameters()
            {
                Curve = ECCurve.NamedCurves.nistP256,
                Q = new ECPoint()
                {
                    X = x,
                    Y = y
                }
            });

            return ecdsa.VerifyData(input, signature.Bytes, HashAlgorithmName.SHA256);
        }
    }
}
