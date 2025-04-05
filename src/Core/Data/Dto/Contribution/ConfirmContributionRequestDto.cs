namespace GamaEdtech.Data.Dto.Contribution
{
    using GamaEdtech.Domain.Enumeration;

    public sealed class ConfirmContributionRequestDto
    {
        public required long ContributionId { get; set; }
        public required long IdentifierId { get; set; }
        public required ContributionType ContributionType { get; set; }
    }
}
