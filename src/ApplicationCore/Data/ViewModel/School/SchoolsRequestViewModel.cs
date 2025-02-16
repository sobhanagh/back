namespace GamaEdtech.Backend.Data.ViewModel.School
{
    using GamaEdtech.Backend.Common.Data;
    using GamaEdtech.Backend.Common.DataAnnotation;

    public sealed class SchoolsRequestViewModel
    {
        [Display]
        public PagingDto? PagingDto { get; set; } = new() { PageFilter = new(), };

        public IEnumerable<int>? CountryIds { get; set; }

        public IEnumerable<int>? StateIds { get; set; }

        public IEnumerable<int>? CityIds { get; set; }
    }
}
