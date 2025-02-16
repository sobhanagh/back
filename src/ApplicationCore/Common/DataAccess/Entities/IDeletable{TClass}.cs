namespace GamaEdtech.Backend.Common.DataAccess.Entities
{
    using GamaEdtech.Backend.Common.DataAnnotation.Schema;

    public interface IDeletable<TClass>
        where TClass : class
    {
        [NotMapped]
        bool Deleted { get; set; }
    }
}
