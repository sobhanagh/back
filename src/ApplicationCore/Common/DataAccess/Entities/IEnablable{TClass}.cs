namespace GamaEdtech.Backend.Common.DataAccess.Entities
{
    using GamaEdtech.Backend.Common.DataAnnotation.Schema;

    public interface IEnablable<TClass>
        where TClass : class
    {
        [NotMapped]
        bool Enabled { get; set; }
    }
}
