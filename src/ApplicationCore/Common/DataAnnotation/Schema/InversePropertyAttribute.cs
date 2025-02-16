namespace GamaEdtech.Backend.Common.DataAnnotation.Schema
{
    using GamaEdtech.Backend.Common.Data;

    public sealed class InversePropertyAttribute(string property) : System.ComponentModel.DataAnnotations.Schema.InversePropertyAttribute(DbProviderFactories.GetFactory.GetObjectName(property))
    {
    }
}