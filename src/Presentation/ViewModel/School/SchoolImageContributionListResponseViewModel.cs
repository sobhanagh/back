namespace GamaEdtech.Presentation.ViewModel.School
{
    public sealed class SchoolImageContributionListResponseViewModel
    {
        public long Id { get; set; }
        public string? CreationUser { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public long SchoolId { get; set; }
    }
}
