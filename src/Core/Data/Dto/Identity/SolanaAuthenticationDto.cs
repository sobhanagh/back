namespace GamaEdtech.Data.Dto.Identity
{
    public class GenerateSolanaChallengeDto
    {
        public required string WalletAddress { get; set; }
    }
    public class SolanaChallengeResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public string Nonce { get; set; } = string.Empty;
    }
    public class SolanaAuthenticationDto
    {
        public required string WalletAddress { get; set; }
        public required string Signature { get; set; }
        public required string Message { get; set; }
        public required string Nonce { get; set; }
    }
}
