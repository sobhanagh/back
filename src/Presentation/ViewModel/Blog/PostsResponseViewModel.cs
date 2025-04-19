namespace GamaEdtech.Presentation.ViewModel.Blog
{
    public sealed class PostsResponseViewModel
    {
        public long Id { get; set; }
        public string? Title { get; set; }
        public string? Summary { get; set; }
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
    }
}
