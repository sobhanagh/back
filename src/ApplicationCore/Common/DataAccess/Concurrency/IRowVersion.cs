namespace GamaEdtech.Backend.Common.DataAccess.Concurrency
{
    using GamaEdtech.Backend.Common.DataAnnotation.Schema;

    using GamaEdtech.Backend.Common.Data;

    public interface IRowVersion
    {
        [System.ComponentModel.DataAnnotations.ConcurrencyCheck]
        [Column(nameof(RowVersion), DataType.Short)]
        long RowVersion { get; set; }
    }
}
