namespace GamaEdtech.Backend.Common.DataAccess.Audit
{
    using System.Threading.Tasks;

    using GamaEdtech.Backend.Common.DataAnnotation;
    using GamaEdtech.Backend.Common.DataAccess.Specification;
    using GamaEdtech.Backend.Common.Data;

    [Injectable]
    public interface IAuditService
    {
        Task<ResultData<ListDataSource<AuditListDto>>> GetAuditsAsync(ListRequestDto<Audit>? requestDto = null);

        Task<ResultData<AuditDto>> GetAsync(ISpecification<Audit> specification);
    }
}
