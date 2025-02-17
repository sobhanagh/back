namespace GamaEdtech.Backend.Common.DataAccess.Entities
{
    using GamaEdtech.Backend.Common.DataAnnotation.Schema;

    public interface IDeletable
    {
        [NotMapped]
        bool Deleted { get; set; }
    }
}
