namespace GamaEdtech.Presentation.ViewModel.Identity
{
    using System.ComponentModel.DataAnnotations;

    public class SolanaChallengeRequestViewModel
    {
        [Required]
        public string? WalletAddress { get; set; }
    }
}
