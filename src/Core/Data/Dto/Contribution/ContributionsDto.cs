namespace GamaEdtech.Data.Dto.Contribution
{
    using GamaEdtech.Domain.Enumeration;

    public sealed class ContributionsDto
    {
        public long Id { get; set; }
        public string? Comment { get; set; }
        public ContributionType? ContributionType { get; set; }
        public Status? Status { get; set; }
        public long? IdentifierId { get; set; }
    }
}
