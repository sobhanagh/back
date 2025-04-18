namespace GamaEdtech.Data.Dto.Blog
{
    public sealed class PostDto
    {
        public string? Title { get; set; }
        public string? Summary { get; set; }
        public string? Body { get; set; }
        public Uri? ImageUri { get; set; }
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
    }
}
