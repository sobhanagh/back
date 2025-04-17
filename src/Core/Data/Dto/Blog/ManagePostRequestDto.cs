namespace GamaEdtech.Data.Dto.Blog
{
    using Microsoft.AspNetCore.Http;

    public sealed class ManagePostRequestDto
    {
        public long? Id { get; set; }
        public string? Title { get; set; }
        public string? Summary { get; set; }
        public string? Body { get; set; }
        public IFormFile? Image { get; set; }
    }
}
