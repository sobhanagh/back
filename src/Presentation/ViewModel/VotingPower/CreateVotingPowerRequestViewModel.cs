namespace GamaEdtech.Presentation.ViewModel.VotingPower
{
    using GamaEdtech.Common.DataAnnotation;

    public sealed class CreateVotingPowerRequestViewModel
    {
        [Display]
        [Required]
        public IEnumerable<VotingPowerViewModel> Data { get; set; }
    }
}
