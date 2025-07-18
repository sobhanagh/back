namespace GamaEdtech.Presentation.ViewModel.Identity
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    public class SolanaAuthenticationViewModel
    {
        [Required]
        [Display]
        [JsonPropertyName("walletAddress")]
        public string WalletAddress { get; set; }

        [Required]
        [Display]
        [JsonPropertyName("signature")]
        public string Signature { get; set; }

        [Required]
        [Display]
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [Required]
        [Display]
        [JsonPropertyName("nonce")]
        public string Nonce { get; set; }
    }
}
