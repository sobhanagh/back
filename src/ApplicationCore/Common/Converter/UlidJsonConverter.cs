namespace GamaEdtech.Backend.Common.Converter
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    using GamaEdtech.Backend.Common.Resources;

    using NUlid;

    public class UlidJsonConverter : JsonConverter<Ulid>
    {
        public override Ulid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.TokenType != JsonTokenType.String
                ? throw new ArgumentException("TokenType")
                : Ulid.TryParse(reader.GetString() ?? string.Empty, out var ulid)
                ? ulid
                : throw new ArgumentException(string.Format(GlobalResource.Validation_ValueIsInvalidAccessor, reader.GetString()));

        public override void Write([NotNull] Utf8JsonWriter writer, Ulid value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());

        public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(Ulid);
    }
}
