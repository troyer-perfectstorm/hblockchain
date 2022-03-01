namespace HBlockchain
{
    public record struct Hash256
    {
        public const int Length = 32;

        public byte[] Bytes { get; }

        public Hash256(byte[] bytes)
        {
            if (bytes == null || bytes.Length != Length)
                throw new ArgumentException($"{nameof(Hash256)} must be {Length} bytes long");

            Bytes = new byte[Length];
            Array.Copy(bytes, Bytes, Length);
        }

        public override string ToString()
        {
            return Convert.ToBase64String(Bytes);
        }
    }
}
