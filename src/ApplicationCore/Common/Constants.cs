
namespace GamaEdtech.Backend.Common
{
    using System.Text.Json.Serialization;

    using Farsica.Framework.Converter;

    public static class Constants
    {
        public const string UtcTimeZoneId = "Coordinated Universal Time";

        [JsonConverter(typeof(EnumStringConverter<EntityType>))]
        public enum EntityType
        {
            ApplicationSetting = 0,
            ApplicationUser = 1,
            ApplicationUserClaim = 2,
        }
    }
}
