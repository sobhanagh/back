namespace GamaEdtech.Backend.Common.Data
{
    using GamaEdtech.Backend.Common.DataAccess.Specification;

    public class ListRequestDto<T>
    {
        public ISpecification<T>? Specification { get; set; }

        public PagingDto? PagingDto { get; set; }
    }
}
