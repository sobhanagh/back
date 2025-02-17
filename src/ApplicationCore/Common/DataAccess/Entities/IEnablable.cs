namespace GamaEdtech.Backend.Common.DataAccess.Entities
{
    using GamaEdtech.Backend.Common.DataAnnotation.Schema;

    public interface IEnablable
    {
        [NotMapped]
        bool Enabled { get; set; }
    }
}
