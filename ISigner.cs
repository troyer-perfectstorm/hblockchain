using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBlockchain
{
    public interface ISigner
    {
        Hash512 Sign(ReadOnlySpan<byte> bytes);
    }
}
