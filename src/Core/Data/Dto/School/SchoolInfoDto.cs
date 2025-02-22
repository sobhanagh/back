namespace GamaEdtech.Data.Dto.School
{
    public sealed class SchoolInfoDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? CityTitle { get; set; }
        public string? CountryTitle { get; set; }
        public string? StateTitle { get; set; }
        public bool HasWebSite { get; set; }
        public bool HasPhoneNumber { get; set; }
        public bool HasEmail { get; set; }
        public DateTimeOffset LastModifyDate { get; set; }
        public double? Score { get; set; }
    }
}
