namespace GamaEdtech.Backend.Data.ViewModel.File
{
    using GamaEdtech.Backend.Common.DataAnnotation;

    using Microsoft.AspNetCore.Http;

    public sealed class CreateFileRequestViewModel
    {
        [Display]
        [Required]
        [FileExtensions("doc,docx,ppt,pptx,pdf")]
        public IFormFile? File { get; set; }
    }
}
