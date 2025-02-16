namespace GamaEdtech.Backend.Common.DataAnnotation.Schema
{
    using GamaEdtech.Backend.Common.Data;

    public sealed class ForeignKeyAttribute(string name) : System.ComponentModel.DataAnnotations.Schema.ForeignKeyAttribute(DbProviderFactories.GetFactory.GetObjectName(name))
    {
    }
}