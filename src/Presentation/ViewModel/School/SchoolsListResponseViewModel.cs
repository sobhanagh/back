namespace GamaEdtech.Presentation.ViewModel.School
{
    public sealed class SchoolsListResponseViewModel
    {
        public string? Name { get; set; }

        public string? Slug { get; set; }

        public string? CoverImage { get; set; }

        public DateTimeOffset LastModifyDate { get; set; }

        public bool WebSite { get; set; }

        public bool Email { get; set; }

        public bool PhoneNumber { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public double? Rate { get; set; }
    }
}
