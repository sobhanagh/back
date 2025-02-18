namespace GamaEdtech.Application.Interface
{
    using GamaEdtech.Common.Data;
    using GamaEdtech.Common.DataAccess.Specification;
    using System.Diagnostics.CodeAnalysis;

    using GamaEdtech.Common.DataAnnotation;

    using GamaEdtech.Data.Dto.School;
    using GamaEdtech.Domain.Entity;

    [Injectable]
    public interface ISchoolService
    {
        Task<ResultData<ListDataSource<SchoolsDto>>> GetSchoolsAsync(ListRequestDto<School>? requestDto = null);
        Task<ResultData<SchoolDto>> GetSchoolAsync([NotNull] ISpecification<School> specification);
        Task<ResultData<int>> ManageSchoolAsync([NotNull] ManageSchoolRequestDto requestDto);
        Task<ResultData<bool>> RemoveSchoolAsync([NotNull] ISpecification<School> specification);
    }
}
