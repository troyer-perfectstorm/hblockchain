using HBlockchain;

// Create genesis account keys
var keys = ECSigner.GenerateKeys();
var genesisSigner = new ECSigner(keys);

var genesisAccount = Account.Create(keys.PublicKey);
var genesisBlock = new Block(Array.Empty<Transaction>())
{
    Hash = new Hash256(Enumerable.Repeat<byte>(1, 32).ToArray()),
    Index = 0,
    Timestamp = new DateTime(2022, 2, 27),
};
const int startingTokens = 100;

// Start the blockchain
var blockchain = new Blockchain(new ECVerifier(), genesisBlock, genesisAccount, startingTokens);

if (blockchain.GetTokenBalance(genesisAccount.Id) != startingTokens)
    throw new Exception("Failed starting tokens");

// Create first recipient account
var aliceSigner = new ECSigner(ECSigner.GenerateKeys());
var aliceAccount = Account.Create(aliceSigner.PublicKey);
blockchain.AddAccount(aliceAccount);

// Start a transaction to send tokens from genesis account to alice
const int tokensToSend = 5;
if (!blockchain.TryAddPendingTransaction(Transaction.Create(genesisAccount.Id, tokensToSend, aliceAccount.Id, genesisSigner)))
    throw new Exception("Failed to add transaction");

// Burn CPU trying to make a new block
var newBlock = blockchain.FinalizePending();
if (!blockchain.TryAddBlock(newBlock))
    throw new Exception("Failed to add new block");

// Verify new amount
if (blockchain.GetTokenBalance(genesisAccount.Id) != startingTokens - tokensToSend)
    throw new Exception("genesis account incorrectly deducted");
if (blockchain.GetTokenBalance(aliceAccount.Id) != tokensToSend)
    throw new Exception("alice account incorrectly granted");

Console.WriteLine("Complete");