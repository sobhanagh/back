namespace GamaEdtech.Data.ViewModel.Identity
{
    using GamaEdtech.Data.Enumeration;

    using GamaEdtech.Common.Converter;

    using System.Text.Json.Serialization;

    public sealed class AuthenticationResponseViewModel
    {
        [JsonConverter(typeof(FlagsEnumerationConverter<Role>))]
        public Role? Roles { get; set; }
    }
}
