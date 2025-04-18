namespace GamaEdtech.Data.Dto.Contribution
{
    using GamaEdtech.Domain.Enumeration;

    public sealed class ConfirmContributionRequestDto
    {
        public required long ContributionId { get; set; }
        public long? IdentifierId { get; set; }
        public required CategoryType CategoryType { get; set; }
    }
}
