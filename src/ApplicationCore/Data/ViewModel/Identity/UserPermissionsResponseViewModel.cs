namespace GamaEdtech.Data.ViewModel.Identity
{
    using GamaEdtech.Data.Enumeration;

    using GamaEdtech.Common.Converter;

    using System.Text.Json.Serialization;

    public sealed class UserPermissionsResponseViewModel
    {
        [JsonConverter(typeof(FlagsEnumerationConverter<Role>))]
        public Role? Roles { get; set; }

        public IEnumerable<ClaimsResponseViewModel>? Claims { get; set; }
    }
}
