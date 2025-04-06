namespace GamaEdtech.Data.Dto.School
{
    using GamaEdtech.Domain.Enumeration;

    public sealed class ManageSchoolContributionRequestDto
    {
        public long? Id { get; set; }
        public int UserId { get; set; }
        public long SchoolId { get; set; }
        public required Status Status { get; set; }
        public required SchoolContributionDto Data { get; set; }
    }
}
