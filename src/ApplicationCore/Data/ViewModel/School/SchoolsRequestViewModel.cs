namespace GamaEdtech.Backend.Data.ViewModel.School
{
    using Farsica.Framework.Data;
    using Farsica.Framework.DataAnnotation;

    public sealed class SchoolsRequestViewModel
    {
        [Display]
        public PagingDto? PagingDto { get; set; } = new() { PageFilter = new(), };

        public IEnumerable<int>? CountryIds { get; set; }

        public IEnumerable<int>? StateIds { get; set; }

        public IEnumerable<int>? CityIds { get; set; }
    }
}
