namespace GamaEdtech.Presentation.ViewModel.School
{
    using System.Text.Json.Serialization;

    using GamaEdtech.Common.Converter;
    using GamaEdtech.Common.DataAnnotation;
    using GamaEdtech.Domain.Enumeration;

    using Microsoft.AspNetCore.Mvc;

    public sealed class CreateSchoolImageRequestViewModel
    {
        [Display]
        [Required]
        [JsonConverter(typeof(EnumerationConverter<FileType, byte>))]
        [FromQuery]
        public FileType? FileType { get; set; }
    }
}
