using System.Diagnostics;

namespace HBlockchain
{
    public class Blockchain
    {
        private IVerifier Verifier { get; }
        private Dictionary<string, int> HashToIndexMap { get; } = new Dictionary<string, int>();
        private List<Block> Blocks { get; } = new List<Block>();

        private Dictionary<string, Account> Accounts { get; } = new Dictionary<string, Account>();
        private Dictionary<string, int> TokenBalance { get; } = new Dictionary<string, int>();

        private List<Transaction> PendingTransactions { get; } = new List<Transaction>();

        public Blockchain(IVerifier verifier, Block genesisBlock, Account genesisAccount, int startingTokens)
        {
            Verifier = verifier;
            Blocks.Add(genesisBlock);
            HashToIndexMap[genesisBlock.Hash.ToString()] = genesisBlock.Index;
            Accounts[genesisAccount.Id.ToString()] = genesisAccount;
            TokenBalance[genesisAccount.Id.ToString()] = startingTokens;
        }

        public int GetTokenBalance(Hash256 accountId)
        {
            TokenBalance.TryGetValue(accountId.ToString(), out int value);
            return value;
        }

        public bool TryAddPendingTransaction(Transaction transaction)
        {
            if (!Accounts.TryGetValue(transaction.From.ToString(), out var fromAccount))
                return false;
            if (!Accounts.ContainsKey(transaction.To.ToString()))
                return false;

            // Is it signed correctly?
            if (!transaction.IsValid(fromAccount.PublicKey, Verifier))
                return false;

            // Check if they have enough in their wallet
            var amountTransferring = PendingTransactions.Where(t => t.From == transaction.From).Sum(t => t.Amount);
            amountTransferring += transaction.Amount;

            var existingTokenBalance = TokenBalance[transaction.From.ToString()];
            if (amountTransferring > existingTokenBalance)
                return false;

            PendingTransactions.Add(transaction.Duplicate());
            return true;
        }

        public Block FinalizePending()
        {
            if (PendingTransactions.Count == 0)
                throw new InvalidOperationException("No pending transactions");

            // Note: Not thread safe
            var rng = new Random();
            var transactions = PendingTransactions.ToArray();
            PendingTransactions.Clear();
            var now = DateTime.UtcNow;
            var previous = Blocks[^1];

            Block? newBlock;
            var timer = new Stopwatch();
            timer.Start();
            while (!Block.TryCreate(rng.Next(), now, previous, transactions, out newBlock))
            {
                // Proof of work burning CPU time
            }
            timer.Stop();
            Console.WriteLine($"New block took {timer.ElapsedMilliseconds} ms");

            return newBlock!;
        }

        public bool TryAddBlock(Block block)
        {
            var prev = Blocks[^1];
            if (block.Index != prev.Index + 1)
                return false;
            if (block.PrevHash.ToString() != prev.Hash.ToString())
                return false;

            if (!Block.TryCreate(block.Nonce, block.Timestamp, prev, block.Transactions, out var verifiedBlock))
                return false;

            if (verifiedBlock == null)
                return false;

            if (verifiedBlock.Hash.ToString() != block.Hash.ToString())
                return false;

            Blocks.Add(verifiedBlock);
            HashToIndexMap[verifiedBlock.Hash.ToString()] = verifiedBlock.Index;

            // Modify state
            foreach (var transaction in verifiedBlock.Transactions)
            {
                TokenBalance[transaction.From.ToString()] -= transaction.Amount;
                TokenBalance[transaction.To.ToString()] += transaction.Amount;
            }

            return true;
        }

        public bool AddAccount(Account account)
        {
            string accountId = account.Id.ToString();
            if (Accounts.ContainsKey(accountId))
                return false;

            Accounts[accountId] = account;
            TokenBalance[accountId] = 0;
            return true;
        }
    }
}
