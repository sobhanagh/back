namespace GamaEdtech.Backend.Data.ViewModel.Identity
{
    using System.Text.Json.Serialization;

    using GamaEdtech.Backend.Common.Converter;
    using GamaEdtech.Backend.Common.DataAnnotation;
    using GamaEdtech.Backend.Data.Enumeration;

    public sealed class ManageUserPermissionsRequestViewModel
    {
        [Display]
        public IEnumerable<string>? Claims { get; set; }

        [Display]
        [JsonConverter(typeof(FlagsEnumerationConverter<Role>))]
        public Role? Roles { get; set; }
    }
}
