using System.Security.Cryptography;
using System.Text;

namespace HBlockchain
{
    public record Block
    {
        public const int Difficulty = 2; // Change to increase time between blocks

        public Hash256 PrevHash { get; set; }
        public Hash256 Hash { get; set; }
        public int Nonce { get; set; }

        public int Index { get; set; }
        public DateTime Timestamp { get; set; }
        public Transaction[] Transactions { get; }

        public Block(Transaction[] transactions)
        {
            Transactions = transactions;
        }

        public static bool TryCreate(int nonce, DateTime nowUtc, Block prev, Transaction[] transactions, out Block? newBlock)
        {
            newBlock = null;
            int nextIndex = prev.Index + 1;

            var sb = new StringBuilder();
            foreach (var transaction in transactions)
            {
                sb.Append(transaction.Hash);
            }
            sb.Append(nonce);
            sb.Append(nextIndex);
            sb.Append(prev.Hash);
            sb.Append(nowUtc);
            var newHash = SHA256.HashData(Encoding.UTF8.GetBytes(sb.ToString()));

            for (int i = 0; i < Difficulty; i++)
            {
                if (newHash[i] != 0)
                    return false;
            }

            // TODO: Transactions should be cloned
            newBlock = new Block(transactions)
            {
                PrevHash = prev.Hash,
                Hash = new Hash256(newHash),
                Nonce = nonce,
                Index = nextIndex,
                Timestamp = nowUtc
            };
            return true;
        }
    }
}
