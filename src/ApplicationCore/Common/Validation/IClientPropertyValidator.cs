namespace GamaEdtech.Backend.Common.Validation
{
    using System.Reflection;

    public interface IClientPropertyValidator
    {
        string? GetJsonMetaData(PropertyInfo? property);
    }
}
