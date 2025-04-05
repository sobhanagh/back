namespace GamaEdtech.Data.Dto.Contribution
{
    using GamaEdtech.Domain.Enumeration;

    public sealed class ContributionDto
    {
        public long Id { get; set; }
        public string? Comment { get; set; }
        public string? Data { get; set; }
        public long? IdentifierId { get; set; }
        public ContributionType? ContributionType { get; set; }
    }
}
