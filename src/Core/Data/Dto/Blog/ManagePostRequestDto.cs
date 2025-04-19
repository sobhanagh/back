namespace GamaEdtech.Data.Dto.Blog
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Http;

    public sealed class ManagePostRequestDto
    {
        public long? Id { get; set; }
        public string? Title { get; set; }
        public string? Summary { get; set; }
        public string? Body { get; set; }
        public IFormFile? Image { get; set; }
        public IEnumerable<int>? Tags { get; set; }
    }
}
