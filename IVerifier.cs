using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBlockchain
{
    public interface IVerifier
    {
        bool IsValidSignature(Hash512 publicKey, ReadOnlySpan<byte> input, Hash512 signature);
    }
}
