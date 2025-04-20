namespace GamaEdtech.Presentation.ViewModel.Blog
{
    using System;

    using GamaEdtech.Domain.Enumeration;

    public sealed class PostContributionListResponseViewModel
    {
        public long Id { get; set; }
        public string? Comment { get; set; }
        public Status Status { get; set; }
        public string? CreationUser { get; set; }
        public DateTimeOffset CreationDate { get; set; }
    }
}
